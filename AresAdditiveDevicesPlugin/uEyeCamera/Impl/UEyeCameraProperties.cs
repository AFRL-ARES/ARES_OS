using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Impl
{
  public class UEyeCameraProperties : BasicReactiveObjectDisposable
  {
    private int _cameraId;
    private int _deviceId;
    private string _model;
    private string _serialNo;
    private bool _isAnalysisCamera;
    private CameraType _cameraType = CameraType.Other;

    public int CameraId
    {
      get => _cameraId;
      set => this.RaiseAndSetIfChanged(ref _cameraId, value);
    }

    public int DeviceId
    {
      get => _deviceId;
      set => this.RaiseAndSetIfChanged(ref _deviceId, value);
    }

    public string Model
    {
      get => _model;
      set => this.RaiseAndSetIfChanged(ref _model, value);
    }

    public string SerialNo
    {
      get => _serialNo;
      set => this.RaiseAndSetIfChanged(ref _serialNo, value);
    }

    public bool IsAnalysisCamera
    {
      get => _isAnalysisCamera;
      set => this.RaiseAndSetIfChanged(ref _isAnalysisCamera, value);
    }

    public CameraType CameraType
    {
      get => _cameraType;
      set => this.RaiseAndSetIfChanged(ref _cameraType, value);
    }

  }
}
