using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.UEyeCamera.Impl;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Vms
{
  public class UEyeCameraViewModel : BasicReactiveObjectDisposable
  {
    private IUEyeCamera _camera;
    private int _cameraIndex;
    private bool _autoExposureOn;
    private bool _autoWhiteBalanceOn;
    private bool _isLive;
    private UEyeCameraSettingsViewModel _cameraSettingsViewModel;
    private UEyeCameraSelectionViewModel _cameraSelectionViewModel;
    private bool _useCrosshair;
    private bool _isAnalysis;
    private readonly CameraType[] _camTypes = Enum.GetValues(typeof(CameraType)).Cast<CameraType>().ToArray();

    public UEyeCameraViewModel(IUEyeCameraRepo camRepo)
    {
#if DISCONNECTED
      //      Camera = ServiceLocator.Current.GetInstance<IUEyeCamera>();
      Camera = new Impl.UEyeCamera
      {
        SelectedCamera = new UEyeCameraProperties
        {
          IsAnalysisCamera = true
        }
      };
      camRepo.Add(Camera);
#else
      Camera = camRepo.GetCamera();
#endif
      CameraSettingsViewModel = new UEyeCameraSettingsViewModel(); // TODO: These settings don't seem to actually apply anything
      CameraSelectionViewModel = new UEyeCameraSelectionViewModel(Camera, camRepo);
      PopulateReactiveCommands();
      AddDisposables();
#if !DISCONNECTED
      Camera?.RunCamera();
#endif
      UseCrosshair = true;
    }

    private void PopulateReactiveCommands()
    {
      RunCamera = ReactiveCommand.Create(InitAndRunCamera);
      StopCamera = ReactiveCommand.Create(() => Camera.StopCamera());
      CloseCamera = ReactiveCommand.Create(() => Camera.CloseCamera());
      CaptureVideo = ReactiveCommand.Create(() => Camera.CaptureVideo());
      CaptureImage = ReactiveCommand.CreateFromTask(Capture);
      ShowSettings = ReactiveCommand.Create(ShowCameraSettings);
      AutoExposure = ReactiveCommand.Create(() => Camera.AutoShutter);
      AutoWhiteBalance = ReactiveCommand.Create(() => Camera.AutoWhiteBalance);
      ToggleAnalysis = ReactiveCommand.Create(() =>
      {
        Camera.SelectedCamera.IsAnalysisCamera = !Camera.SelectedCamera.IsAnalysisCamera;
        var cameraTypeIndex = (int)++Camera.SelectedCamera.CameraType;
        cameraTypeIndex %= _camTypes.Length;
        Camera.SelectedCamera.CameraType = _camTypes[cameraTypeIndex];

      });
      ToggleCrosshair = ReactiveCommand.Create(() => { UseCrosshair = !UseCrosshair; });
    }

    public async Task Capture()
    {
      await Camera.CaptureImage();
    }

    private async void InitAndRunCamera()
    {
      _cameraSelectionViewModel.SelectorOpen = true;
      while (_cameraSelectionViewModel.SelectorOpen)
      {
        await Task.Delay(100);
      }

#if !DISCONNECTED
      if (Camera.SelectedCamera != null)
        Camera.RunCamera();
#endif
    }



    private void AddDisposables()
    {
      Disposables.Add(RunCamera);
      Disposables.Add(StopCamera);
      Disposables.Add(CloseCamera);
      Disposables.Add(CaptureVideo);
      Disposables.Add(CaptureImage);
      Disposables.Add(ShowSettings);
      Disposables.Add(AutoExposure);
      Disposables.Add(AutoWhiteBalance);
    }

    private void ShowCameraSettings()
    {
      _cameraSettingsViewModel.SettingsOpen = true;
    }

    public ReactiveCommand<Unit, Unit> RunCamera { get; set; }

    public ReactiveCommand<Unit, Unit> StopCamera { get; set; }

    public ReactiveCommand<Unit, Unit> CloseCamera { get; set; }

    public ReactiveCommand<Unit, Unit> CaptureVideo { get; set; }

    public ReactiveCommand<Unit, Unit> CaptureImage { get; set; }

    public ReactiveCommand<Unit, Unit> ShowSettings { get; set; }

    public ReactiveCommand<Unit, bool> AutoExposure { get; set; }

    public ReactiveCommand<Unit, bool> AutoWhiteBalance { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleAnalysis { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleCrosshair { get; set; }

    public bool AutoExposureOn
    {
      get => _autoExposureOn;
      set => this.RaiseAndSetIfChanged(ref _autoExposureOn, value);
    }

    public bool AutoWhiteBalanceOn
    {
      get => _autoWhiteBalanceOn;
      set => this.RaiseAndSetIfChanged(ref _autoWhiteBalanceOn, value);
    }

    public bool IsLive
    {
      get => _isLive;
      set => this.RaiseAndSetIfChanged(ref _isLive, value);
    }

    public bool UseCrosshair
    {
      get => _useCrosshair;
      set => this.RaiseAndSetIfChanged(ref _useCrosshair, value);
    }

    public bool IsOpened => Camera.SelectedCamera != null;

    public IUEyeCamera Camera
    {
      get => _camera;
      set => this.RaiseAndSetIfChanged(ref _camera, value);
    }

    public UEyeCameraSettingsViewModel CameraSettingsViewModel
    {
      get => _cameraSettingsViewModel;
      set => this.RaiseAndSetIfChanged(ref _cameraSettingsViewModel, value);
    }

    public UEyeCameraSelectionViewModel CameraSelectionViewModel
    {
      get => _cameraSelectionViewModel;
      set => this.RaiseAndSetIfChanged(ref _cameraSelectionViewModel, value);
    }

    public bool IsAnalysis
    {
      get => _isAnalysis;
      set => this.RaiseAndSetIfChanged(ref _isAnalysis, value);
    }

    public int CameraIndex
    {
      get => _cameraIndex;
      set => this.RaiseAndSetIfChanged(ref _cameraIndex, value);
    }

  }
}