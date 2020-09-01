using AresAdditiveDevicesPlugin.UEyeCamera.Impl;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Vms
{
  public class UEyeCameraSelectionViewModel : BasicReactiveObjectDisposable
  {
    private bool _errorTextShown;
    private int _cameraId;
    private int _deviceId;
    private UEyeCameraProperties _selectedCamera;
    private bool _selectorOpen;
    private IUEyeCameraRepo _camRepo;
    private IUEyeCamera _cameraEntity;

    public UEyeCameraSelectionViewModel(IUEyeCamera camera, IUEyeCameraRepo camRepo)
    {
#if DISCONNECTED
      return;
#endif
      CameraEntity = camera;
      _camRepo = camRepo;
      if (camera.SettingsOk)
      {
        camRepo.CollectionChanged += DeviceListChanged;
        DeviceListChanged(this, null);
      }
      else
      {
        ErrorTextShown = true;
      }
      CancelCommand = ReactiveCommand.Create(() => SelectorOpen = false);
      camera.SelectedCamera = SelectedCamera;
      SelectorOpen = false;
    }

    private void DeviceListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      {
        if (!_camRepo.Any())
          return;
        if (SelectedCamera == null)
        {
          var selectedCam = _camRepo.First(camera => !camera.IsAssigned);
          SelectedCamera = selectedCam.SelectedCamera;
          selectedCam.IsAssigned = true;
          SelectedCamera.IsAnalysisCamera = true; // Sets the last (in this case, the second) camera loaded as the AnalysisCamera
        }
      }
    }

    public ReactiveCommand<Unit, Unit> OkCommand { get; set; }

    public ReactiveCommand<Unit, bool> CancelCommand { get; set; }

    public bool SelectorOpen
    {
      get
      {
        return _selectorOpen;
      }

      set
      {
        this.RaiseAndSetIfChanged(ref _selectorOpen, value);
      }
    }

    public bool ErrorTextShown
    {
      get => _errorTextShown;
      set => this.RaiseAndSetIfChanged(ref _errorTextShown, value);
    }

    public int CameraID
    {
      get => _cameraId;
      set => this.RaiseAndSetIfChanged(ref _cameraId, value);

    }

    public int DeviceID
    {
      get => _deviceId;
      set => this.RaiseAndSetIfChanged(ref _deviceId, value);
    }

    public UEyeCameraProperties SelectedCamera
    {
      get => _selectedCamera;
      set => this.RaiseAndSetIfChanged(ref _selectedCamera, value);
    }

    public IUEyeCameraRepo AvailableCameras
    {
      get => _camRepo;
      set => this.RaiseAndSetIfChanged(ref _camRepo, value);
    }

    public IUEyeCamera CameraEntity
    {
      get => _cameraEntity;
      set
      {
        this.RaiseAndSetIfChanged(ref _cameraEntity, value);
        if (value?.SelectedCamera != null)
        {
          SelectedCamera = value.SelectedCamera;
        }
      }
    }
  }
}