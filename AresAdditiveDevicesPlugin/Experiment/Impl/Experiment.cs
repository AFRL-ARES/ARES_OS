using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using AresAdditiveDevicesPlugin.PythonInterop;
using AresAdditiveDevicesPlugin.PythonStageController;
using AresAdditiveDevicesPlugin.PythonStageController.Impl;
using AresAdditiveDevicesPlugin.UEyeCamera;
using AresAdditiveDevicesPlugin.UEyeCamera.Vms;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class Experiment : BasicReactiveObjectDisposable, IExperiment
  {
    private readonly IPythonInvoker _pyInvoker;
    private IExperimentParameters _parameters;
    private double _results;
    private int _number;
    private readonly IUEyeCameraRepo _camRepo;
    private readonly ICampaignSetup _campaignSetup;
    private readonly IStageController _stageController;
    private readonly PythonCommandConfig _pyConfig;

    public Experiment(IPythonInvoker pyInvoker, IUEyeCameraRepo camRepo, ICampaignSetup campaignSetup, IStageController stageController)
    {
      _pyConfig = File.ReadAllText(@"..\..\..\py\PythonCommandConfig.json").DeserializeObject<PythonCommandConfig>();
      _stageController = stageController;
      _pyInvoker = pyInvoker;
      _camRepo = camRepo;
      _campaignSetup = campaignSetup;
    }

    public async Task<bool> Run(IExperimentParameters parameters, BasicUserDefinedComponent analysisComponent)
    {

      // go to next position: find the grid locexperimation
      Parameters = parameters;
      if (!_campaignSetup.Grid.Any(available => available))
      {
        return false;
      }
      var index = await MoveToNextPosition();
#if !DISCONNECTED
      await _stageController.SelectExtruderTool();
      await Task.Delay(250); // This could be unnecessarry at this point

      if (!await CheckCurrentPosition())
      {
        return false; // The stage did not move to an available location.
      }
      await PushExperimentParameters();
      await _stageController.SetHome();
      await _stageController.GenerateToolpath();
      await _pyInvoker.InvokePythonDirect(_pyConfig.RunToolpathCommand);
      await MoveToAnalysisPosition(index);
#endif
#if DISCONNECTED
      index = _campaignSetup.Grid.Count(available => !available) + 1;
#endif
      Application.Current.Dispatcher.Invoke(() => _campaignSetup.Grid[index] = false);
      Number = _campaignSetup.Grid.Count(available => !available); // regardless of position, give a logical experiment number
      await Application.Current.Dispatcher.Invoke(async () => await PerformAnalysis(analysisComponent));


      return true;
    }

    private async Task<int> MoveToNextPosition()
    {
      await _stageController.GetPositions();
      await _stageController.MoveTo(_stageController.XPosition, _stageController.YPosition, _stageController.ZPosition + 2);

      var currentPosition = new System.Windows.Point(_stageController.XPosition, _stageController.YPosition);
      var availableIndex = _campaignSetup.Grid.NextAvailableIndex(currentPosition);
      var experimentStart = _campaignSetup.Grid.GetStartingPointAbsolute(availableIndex);

      await _stageController.MoveTo(experimentStart.X, experimentStart.Y, _campaignSetup.Grid.InitZPosition);
      return availableIndex;
    }

    private async Task<bool> CheckCurrentPosition()
    {
      await _stageController.GetPositions();
      var currentPosition = new System.Windows.Point(_stageController.XPosition, _stageController.YPosition);
      return _campaignSetup.Grid[currentPosition];
    }

    private async Task MoveToAnalysisPosition(int experimentIndex)
    {
      await _stageController.SelectAlignmentTool();
      await _stageController.GetPositions();
      var experimentStart2D = _campaignSetup.Grid.GetStartingPointAbsolute(experimentIndex);

      // Get the location (absolute) of where the Experiment Origin
      var experimentStart = new Point3D(experimentStart2D.X, experimentStart2D.Y, _campaignSetup.Grid.InitZPosition);

      // Get the location (absolute) of the Analysis position
      var analysisPosition = experimentStart + new Vector3D(_campaignSetup.Grid.AnalysisStepX, _campaignSetup.Grid.AnalysisStepY, _campaignSetup.Grid.AnalysisStepZ);

      await _stageController.GetPositions();
      // Move to the Analysis XY at the current Z
      await _stageController.MoveTo(analysisPosition.X, analysisPosition.Y, analysisPosition.Z);
      // Move to the Analysis Z at the current XY

    }

    private async Task PerformAnalysis(BasicUserDefinedComponent analysisComponent)
    {
      Bitmap analysisImage = null;
#if DISCONNECTED
      // Run on what is guaranteed to be a new thread (unlikely the UI thread)
      //analysisImage = await Task.Run( async () => await ViewModelLocator.Kernel.GetAll<CameraViewModel>().FirstOrDefault().Camera.CaptureImage() );
      var camVm = ServiceLocator.Current.GetAllInstances<UEyeCameraViewModel>().FirstOrDefault();
      await Task.Delay(500);
      //      analysisImage = await camVm.Camera.CaptureImage(Number);
#else
      //var analysisCamera = _camRepo.GetCamera(0);
      var analysisCamera = _camRepo.FirstOrDefault(cam => cam.SelectedCamera.IsAnalysisCamera);
      await Task.Delay(1000);
      analysisImage = await analysisCamera.CaptureImage();
#endif

      analysisComponent.AnalysisImage = new Bitmap(analysisImage);
      await analysisComponent.StartComponent(null);
      Results = analysisComponent.Result;
    }

    private async Task PushExperimentParameters()
    {
      // Custom variables
      await WritePyDict("uservars.var1", Parameters[0].VarEntry.Value);
      await WritePyDict("uservars.var2", Parameters[1].VarEntry.Value);
      await WritePyDict("uservars.var3", Parameters[2].VarEntry.Value);
      await WritePyDict("uservars.var4", Parameters[3].VarEntry.Value);
      await WritePyDict("uservars.var5", Parameters[4].VarEntry.Value);
      await WritePyDict("uservars.var6", Parameters[5].VarEntry.Value);

      await WritePyDict("dispenser.prime", Parameters.First(param => param.VarEntry.Name == "dispenser.prime").VarEntry.Value);
      await WritePyDict("dispenser.prime_rate", Parameters.First(param => param.VarEntry.Name == "dispenser.prime_rate").VarEntry.Value);
      await WritePyDict("dispenser.prime_delay", Parameters.First(param => param.VarEntry.Name == "dispenser.prime_delay").VarEntry.Value);
      await WritePyDict("dispenser.retract", Parameters.First(param => param.VarEntry.Name == "dispenser.retract").VarEntry.Value);
      await WritePyDict("dispenser.retract_rate", Parameters.First(param => param.VarEntry.Name == "dispenser.retract_rate").VarEntry.Value);
      await WritePyDict("dispenser.work_dist", Parameters.First(param => param.VarEntry.Name == "dispenser.work_dist").VarEntry.Value);
      await WritePyDict("dispenser.multiplier", Parameters.First(param => param.VarEntry.Name == "dispenser.multiplier").VarEntry.Value);
    }

    private async Task WritePyDict(string dictionaryField, double value)
    {
      var split = dictionaryField.Split('.');
      var dictName = split[0];
      var dictKey = split[1];
      await _pyInvoker.WriteDict(dictName, dictKey, value);
    }

    public IExperimentParameters Parameters
    {
      get => _parameters;
      set => this.RaiseAndSetIfChanged(ref _parameters, value);
    }

    public double Results
    {
      get => _results;
      set => this.RaiseAndSetIfChanged(ref _results, value);
    }

    public int Number
    {
      get => _number;
      set => this.RaiseAndSetIfChanged(ref _number, value);
    }
  }
}
