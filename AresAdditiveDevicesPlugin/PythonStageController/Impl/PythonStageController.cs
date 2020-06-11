using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.PythonInterop;
using AresAdditiveDevicesPlugin.PythonStageController.Commands;
using AresAdditiveDevicesPlugin.Terminal;
using ARESCore.DeviceSupport;
using ARESCore.DeviceSupport.Usb;
using ARESCore.Registries;
using CommonServiceLocator;
using Prism.Ioc;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.PythonStageController.Impl
{
  public class PythonStageController : UsbDevice, IStageController
  {
    private USBDeviceInfo _usbDeviceInfo;
    private readonly IPythonProcessRepository _pythonProcessRepo;
    private readonly IPythonBindings _bindings;
    private readonly IPythonInvoker _pyInvoker;
    private readonly PythonCommandConfig _config;
    private double _xPosition;
    private double _yPosition;
    private double _zPosition;
    private double _ePosition;
    private int _extruderLightIntensity;
    private int _alignmentLightIntensity;
    private bool _isInitialized;
    private double _xystepSizeSetting;
    private int _xyspeedSetting;
    private double _zstepSizeSetting;
    private int _zspeedSetting;
    private IExperimentGrid _experimentGrid;

    public PythonStageController(IPythonInvoker pyInvoker, IPythonProcessRepository pyProcessRepo, IPythonBindings pyBindings, IExperimentGrid experimentGrid)
    {
      ExperimentGrid = experimentGrid;
      var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
      var configDir = currentDir.Parent.Parent.Parent.GetDirectories().First(dirInfo => dirInfo.Name.ToLower().StartsWith("py"));
      var files = configDir.GetFiles();

      var pythonPath = files.First(fileInfo => fileInfo.Name.ToLower().StartsWith("pythoncommandconfig")).FullName;
      _pythonProcessRepo = pyProcessRepo;
      _bindings = pyBindings;
      _pyInvoker = pyInvoker;
      var config = File.ReadAllText(pythonPath).DeserializeObject<PythonCommandConfig>();
      _config = config;
    }

    public override USBDeviceInfo USBDeviceInfo
    {
      get => _usbDeviceInfo;
      set => this.RaiseAndSetIfChanged(ref _usbDeviceInfo, value);
    }
    public async void Init()
    {
      var split = _config.InitCommand.Split('.');
      if (split.Length != 2)
      {
        ErrorText = "Invalid command " + _config.InitCommand + ". Should be something like <classname>.<defname>.";
        return;
      }
      var className = split[0];
      var funcName = split[1];
      // wait for python stuff to load
      var totalTimeout = 10000;
      while (!_pythonProcessRepo.Any(p => p.ClassName.Equals(className) && p.ComponentName.Equals(funcName)))
      {
        await Task.Delay(100);
        totalTimeout -= 100;
        if (totalTimeout < 0)
        {
          ErrorText = "Couldn't load the python init command " + _config.InitCommand;
          return;
        }
      }
      var process = _pythonProcessRepo.First(p => p.ClassName.Equals(className) && p.ComponentName.Equals(funcName));
      // grab the init routine from the config
      try
      {
        const int port = 5005;
        Task.Run(() => ServiceLocator.Current.GetInstance<ITerminal>().Connect(port));
        await _bindings.RunPythonDef(process.ClassName, process.ComponentName, null);
        Observable.Interval(TimeSpan.FromSeconds(2)).Subscribe(async _ => await GetPositions());
        IsInitialized = true;
      }
      catch (Exception)
      {
        ErrorText = "Couldn't run the Python definition " + _config.InitCommand;
        return;
      }
      XYStepSizeSetting = 0.5;
      XYSpeedSetting = 1000;
      ZStepSizeSetting = 0.5;
      ZSpeedSetting = 500;
    }

    public void Activate()
    {

    }

    public string Name { get; set; }

    public IAresDeviceState CurrentState { get; set; }
    public IAresDeviceState TargetState { get; set; }
    public void IssueCommand(IAresDeviceCommand command)
    {
    }

    public void RegisterCommands(IContainerRegistry registry)
    {
      var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
      repo.Add(new StageControllerChangeTipCommand());
      repo.Add(new StageControllerStepXCommand());
      repo.Add(new StageControllerCleanTipCommand());
      repo.Add(new StageControllerEStopCommand());
      repo.Add(new StageControllerGenerateToolpathCommand());
      repo.Add(new StageControllerGetEPositionCommand());
      repo.Add(new StageControllerGetZPositionCommand());
      repo.Add(new StageControllerGetYPositionCommand());
      repo.Add(new StageControllerGetXPositionCommand());
      repo.Add(new StageControllerGetPositionsCommand());
      repo.Add(new StageControllerGoHomeCommand());
      repo.Add(new StageControllerHomeCommand());
      repo.Add(new StageControllerInitCommand());
      repo.Add(new StageControllerLevelCommand());
      repo.Add(new StageControllerMaintenancePositionCommand());
      repo.Add(new StageControllerMoveToCommand());
      repo.Add(new StageControllerOffsetDefinitionCommand());
      repo.Add(new StageControllerRemoveSampleCommand());
      repo.Add(new StageControllerSelectToolCommand());
      repo.Add(new StageControllerSetExtruderLightIntensityCommand());
      repo.Add(new StageControllerSetHomeCommand());
      repo.Add(new StageControllerSetXYSpeedCommand());
      repo.Add(new StageControllerSetZSpeedCommand());
      repo.Add(new StageControllerStepNorthEastCommand());
      repo.Add(new StageControllerStepNorthWestCommand());
      repo.Add(new StageControllerStepSouthEastCommand());
      repo.Add(new StageControllerStepSouthWestCommand());
      repo.Add(new StageControllerStepYCommand());
      repo.Add(new StageControllerStepZCommand());
      repo.Add(new StageControllerSetAlignmentLightIntensityCommand());
      repo.Add(new StageControllerGotoNextAvailableExperimentCommand());
      repo.Add(new StageControllerValidateExperimentCellAvailability());
      repo.Add(new StageControllerRunToolpathCommand());
      repo.Add(new StageControllerGotoAnalysisPositionCommand());
      repo.Add(new StageControllerInvalidateExperimentCellCommand());
      repo.Add(new StageControllerSetNozzleDiameterCommand());
      repo.Add(new StageControllerSetExtrusionMultiplierCommand());
      repo.Add(new StageControllerSetPrimeDistanceCommand());
      repo.Add(new StageControllerSetPrimeDelayCommand());
      repo.Add(new StageControllerSetPrimeRateCommand());
      repo.Add(new StageControllerSetRetractDistanceCommand());
      repo.Add(new StageControllerSetRetractDelayCommand());
      repo.Add(new StageControllerSetRetractRateCommand());
      repo.Add(new StageControllerSetTipHeightCommand());
      repo.Add(new StageControllerSetBedTemperatureCommand());
      repo.Add(new StageControllerSetVar1Command());
      repo.Add(new StageControllerSetVar2Command());
      repo.Add(new StageControllerSetVar3Command());
      repo.Add(new StageControllerSetVar4Command());
      repo.Add(new StageControllerSetVar5Command());
      repo.Add(new StageControllerSetVar6Command());
      repo.Add(new StageControllerSetDispenseSpeedCommand());
    }

    public async Task WritePyDict(string dictionaryField, double value)
    {
      var split = dictionaryField.Split('.');
      var dictName = split[0];
      var dictKey = split[1];
      try
      {

        await _pyInvoker.WriteDict(dictName, dictKey, value);
      }
      catch
      {
      }
    }

    public async Task WritePyDict(string dictionaryField, string value)
    {
      var split = dictionaryField.Split('.');
      var dictName = split[0];
      var dictKey = split[1];
      try
      {

        await _pyInvoker.WriteDict(dictName, dictKey, value);
      }
      catch
      {
      }
    }

    public Task Abort()
    {
      return _pyInvoker.InvokePythonDirect(_config.AbortCommand);
    }

    public string GetSampleScriptEntry()
    {
      return string.Empty;
    }

    public int ReadRate { get; set; }
    public string Validate()
    {
      return string.Empty;
    }


    public async Task StepX(double dist)
    {
      if (dist > 0)
      {
        await _pyInvoker.InvokePythonDirect(_config.XPlusTranslateCommand);
        return;
      }

      await _pyInvoker.InvokePythonDirect(_config.XMinusTranslateCommand);
#if DISCONNECTED
      XPosition += dist;
#endif
    }

    public async Task SetHome()
    {
      await _pyInvoker.InvokePythonDirect(_config.SetHomeCommand);
    }

    public async Task StepY(double dist)
    {
      if (dist > 0)
        await _pyInvoker.InvokePythonDirect(_config.YPlusTranslateCommand);
      else
        await _pyInvoker.InvokePythonDirect(_config.YMinusTranslateCommand);

#if DISCONNECTED
      YPosition += dist;
#endif
    }

    public async Task StepZ(double dist)
    {
      if (dist > 0)
        await _pyInvoker.InvokePythonDirect(_config.ZPlusTranslateCommand);
      else
        await _pyInvoker.InvokePythonDirect(_config.ZMinusTranslateCommand);

#if DISCONNECTED
      ZPosition += dist;
#endif
    }

    public string ErrorText { get; set; }

    public double XPosition
    {
      get => _xPosition;
      set => this.RaiseAndSetIfChanged(ref _xPosition, value);
    }

    public double YPosition
    {
      get => _yPosition;
      set => this.RaiseAndSetIfChanged(ref _yPosition, value);
    }

    public double ZPosition
    {
      get => _zPosition;
      set => this.RaiseAndSetIfChanged(ref _zPosition, value);
    }

    public double EPosition
    {
      get => _ePosition;
      set => this.RaiseAndSetIfChanged(ref _ePosition, value);
    }

    public int ExtruderLightIntensity
    {
      get => _extruderLightIntensity;
      set
      {
        this.RaiseAndSetIfChanged(ref _extruderLightIntensity, value);
        _pyInvoker.InvokePythonDirect(_config.ProcessLightsCommand, new List<object> { value }).Wait();
      }
    }

    public int AlignmentLightIntensity
    {
      get => _alignmentLightIntensity;
      set
      {
        this.RaiseAndSetIfChanged(ref _alignmentLightIntensity, value);
        _pyInvoker.InvokePythonDirect(_config.AlighmentLightsCommand, new List<object> { value }).Wait();
      }
    }

    public bool IsInitialized
    {
      get => _isInitialized;
      set => this.RaiseAndSetIfChanged(ref _isInitialized, value);
    }

    public double XYStepSizeSetting
    {
      get => _xystepSizeSetting;
      set
      {
        this.RaiseAndSetIfChanged(ref _xystepSizeSetting, value);
        var args = new List<object>
        {
          value
        };
        _pyInvoker.InvokePythonDirect(_config.TranslateIncrementCommand, args).Wait();
      }
    }

    public int XYSpeedSetting
    {
      get => _xyspeedSetting;
      set
      {
        this.RaiseAndSetIfChanged(ref _xyspeedSetting, value);
        var args = new List<object>
        {
          value
        };
        _pyInvoker.InvokePythonDirect(_config.XYSpeedCommand, args).Wait();
      }
    }

    public double ZStepSizeSetting
    {
      get => _zstepSizeSetting;
      set
      {
        this.RaiseAndSetIfChanged(ref _zstepSizeSetting, value);
        var args = new List<object>
        {
          value
        };
        _pyInvoker.InvokePythonDirect(_config.TranslateIncrementCommand, args).Wait();
      }
    }

    public int ZSpeedSetting
    {
      get => _zspeedSetting;
      set
      {
        this.RaiseAndSetIfChanged(ref _zspeedSetting, value);
        var args = new List<object>
        {
          value
        };
        _pyInvoker.InvokePythonDirect(_config.XYSpeedCommand, args).Wait();
      }
    }

    public async Task Home()
    {
      await Task.Run(async () => await
       _pyInvoker.InvokePythonDirect(_config.HomingCommand));
    }

    public async Task RemoveSample()
    {
      await Task.Run(async () => await
      _pyInvoker.InvokePythonDirect(_config.RemoveSampleCommand));
    }

    public async Task ChangeTip()
    {
      await _pyInvoker.InvokePythonDirect(_config.ChangeTipCommand)
              ;
    }

    public async Task CleanTip()
    {
      await _pyInvoker.InvokePythonDirect(_config.CleanTipCommand);
    }

    public async Task Prime(double distance, double rate)
    {
      await _pyInvoker.InvokePythonDirect(_config.ExtrudeCommand, new List<object> { -distance, rate });
    }

    public async Task Retract(double distance, double rate)
    {
      await _pyInvoker.InvokePythonDirect(_config.ExtrudeCommand, new List<object> { distance, rate });
    }

    public async Task GenerateToolpath()
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var cfgSettings = config.AppSettings.Settings;
      var toolpathPath = cfgSettings["Toolpath"].Value;

      await Task.Run(async () => await
     _pyInvoker.InvokePythonDirect(_config.GenerateToolpathCommand, new List<object> { toolpathPath }));
    }

    public async Task RunToolpath()
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var toolpathFilePath = config.AppSettings.Settings["Toolpath"].Value;
      if (File.Exists(toolpathFilePath))
      {
        await Task.Run(async () => await _pyInvoker.InvokePythonDirect(_config.RunToolpathCommand));
      }
    }

    public async Task OffsetDefinition()
    {
      // Assume local host for now
      await Task.Run(async () => await _pyInvoker.InvokePythonDirect(_config.OffsetDefinitionCommand, new List<object> { 5005 }));
    }


    public async Task Level()
    {
      await Task.Run(async () => await
      _pyInvoker.InvokePythonDirect(_config.BedLevelingCommand));
    }

    public async Task MaintenancePosition()
    {
      await Task.Run(async () => await
      _pyInvoker.InvokePythonDirect(_config.MaintenancePositionCommand));
    }

    public async Task EStop()
    {
      await _pyInvoker.InvokePythonDirect(_config.EmergencyStop);
    }

    public async Task GoHome()
    {
      await Task.Run(async () => await
      _pyInvoker.InvokePythonDirect(_config.GoHome));
    }

    public async Task StepSouthEast(double dist)
    {
      await _pyInvoker.InvokePythonDirect(_config.SoutheastTranslateCommand);
    }

    public async Task StepNorthEast(double dist)
    {
      await _pyInvoker.InvokePythonDirect(_config.NortheastTranslateCommand);
    }

    public async Task StepSouthWest(double dist)
    {
      await _pyInvoker.InvokePythonDirect(_config.SouthwestTranslateCommand);
    }

    public async Task StepNorthWest(double dist)
    {
      await _pyInvoker.InvokePythonDirect(_config.NorthwestTranslateCommand);
    }

    public async Task TranslateX(double dist)
    {
#if !DISCONNECTED

      if ( dist < 0 )
      {
        await _pyInvoker.InvokePythonDirect( _config.TranslateCommand, new List<object> { "W", dist, 5000 } );
      }
      else
      {
        await _pyInvoker.InvokePythonDirect( _config.TranslateCommand, new List<object> { "E", dist, 5000 } );

      }
#endif
    }

    public async Task TranslateY(double dist)
    {
#if !DISCONNECTED
      if ( dist < 0 )
      {
        await _pyInvoker.InvokePythonDirect( _config.TranslateCommand, new List<object> { "S", dist, 5000 } );
      }
      else
      {
        await _pyInvoker.InvokePythonDirect( _config.TranslateCommand, new List<object> { "N", dist, 5000 } );
      }
#endif
    }

    public async Task TranslateZ(double dist)
    {
#if !DISCONNECTED
      if ( dist < 0 )
      {
        await _pyInvoker.InvokePythonDirect( _config.TranslateCommand, new List<object> { "D", -dist, 500 } );
      }
      else
      {
        await _pyInvoker.InvokePythonDirect( _config.TranslateCommand, new List<object> { "U", dist, 500 } );
      }
#endif
    }

    public async Task MoveTo(float x, float y, float z)
    {
#if !DISCONNECTED
      await _pyInvoker.InvokePythonDirect( _config.MoveToCommand, new List<object> { $"{x},{y},{z}" } );
#endif
#if DISCONNECTED
      XPosition = x;
      YPosition = y;
      ZPosition = z;
      await Task.CompletedTask;
#endif
    }


    public async Task MoveTo(double x, double y, double z)
    {
      await MoveTo((float)x, (float)y, (float)z); // python uses float
    }

    public async Task SelectAlignmentTool()
    {
      await _pyInvoker.InvokePythonDirect(_config.AlignmentCameraToolCommand);
      await Task.Delay(4000);

    }

    public async Task SelectExtruderTool()
    {
      await _pyInvoker.InvokePythonDirect(_config.DepositionHeadToolCommand);
      await Task.Delay(4000);
    }

    public async Task GetPositions()
    {
      try
      {
#if !DISCONNECTED
        await _pyInvoker.InvokePythonDirect(_config.XPosition);
        XPosition = Convert.ToDouble(_bindings.LastResult);
        await _pyInvoker.InvokePythonDirect(_config.YPosition);
        YPosition = Convert.ToDouble(_bindings.LastResult);
        await _pyInvoker.InvokePythonDirect(_config.ZPosition);
        ZPosition = Convert.ToDouble(_bindings.LastResult);
        await _pyInvoker.InvokePythonDirect(_config.EPosition);
        EPosition = Convert.ToDouble(_bindings.LastResult);
#endif

      }
      catch (Exception)
      {
        // meh...
      }
    }

    public IExperimentGrid ExperimentGrid
    {
      get => _experimentGrid;
      private set => this.RaiseAndSetIfChanged(ref _experimentGrid, value);
    }

    public async Task GenerateGridExtents()
    {
#if !DISCONNECTED
      await GenerateToolpath();
      ExperimentGrid.ExtentX = await _bindings.GetDict("tpGen", "maxcoords", 0);
      ExperimentGrid.ExtentY = await _bindings.GetDict("tpGen", "maxcoords", 1);
      ExperimentGrid.ExtentZ = await _bindings.GetDict("tpGen", "maxcoords", 2);
      ExperimentGrid.LimitWest = await _bindings.GetDict("coords", "lim_west");
      ExperimentGrid.LimitEast = await _bindings.GetDict("coords", "lim_east");
      ExperimentGrid.LimitSouth = await _bindings.GetDict("coords", "lim_south");
      ExperimentGrid.LimitNorth = await _bindings.GetDict("coords", "lim_north");
#else
      ExperimentGrid.ExtentX = 10;
      ExperimentGrid.ExtentY = 13;
      ExperimentGrid.ExtentZ = 0;
      ExperimentGrid.LimitWest = 10;
      ExperimentGrid.LimitEast = 200;
      ExperimentGrid.LimitNorth = 230;
      ExperimentGrid.LimitSouth = -24;

      if (ExperimentGrid.InitXPosition < ExperimentGrid.LimitWest)
        ExperimentGrid.InitXPosition = ExperimentGrid.LimitWest;
      if (ExperimentGrid.InitYPosition < ExperimentGrid.LimitSouth)
        ExperimentGrid.InitYPosition = ExperimentGrid.LimitSouth;

      if (!ExperimentGrid.Any())
      {
        await MoveTo(ExperimentGrid.LimitWest, ExperimentGrid.LimitSouth, 3.14);
      }
#endif

      ExperimentGrid.GenerateGrid();
    }

    public void SetInitialPositionAt(int index)
    {
      var xyPos = ExperimentGrid.GetStartingPointIdle(index);
      ExperimentGrid.InitXPosition = xyPos.X;
      ExperimentGrid.InitYPosition = xyPos.Y;
    }


    public void SetTopLeft()
    {
      ExperimentGrid.InitXPosition = ExperimentGrid.LimitWest;
      ExperimentGrid.InitYPosition = ExperimentGrid.LimitNorth;
    }

    public void SetTopRight()
    {
      ExperimentGrid.InitXPosition = ExperimentGrid.LimitEast;
      ExperimentGrid.InitYPosition = ExperimentGrid.LimitNorth;
    }

    public void SetBottomLeft()
    {
      ExperimentGrid.InitXPosition = ExperimentGrid.LimitWest;
      ExperimentGrid.InitYPosition = ExperimentGrid.LimitSouth;
    }

    public void SetBottomRight()
    {
      ExperimentGrid.InitXPosition = ExperimentGrid.LimitEast;
      ExperimentGrid.InitYPosition = ExperimentGrid.LimitSouth;
    }
  }
}
