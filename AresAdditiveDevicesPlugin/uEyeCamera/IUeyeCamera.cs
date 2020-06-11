using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AresAdditiveDevicesPlugin.UEyeCamera.Impl;
using ARESCore.DisposePatternHelpers;

namespace AresAdditiveDevicesPlugin.UEyeCamera
{
  public interface IUEyeCamera : IBasicReactiveObjectDisposable
  {
    ulong CaptureStatus { get; set; }
    double FPS { get; set; }
    string CameraName { get; set; }
    bool AutoShutter { get; set; }
    bool AutoWhiteBalance { get; set; }
    BitmapImage CurrentImage { get; set; }
    UEyeCameraProperties SelectedCamera { get; set; }
    bool IsLive { get; set; }
    bool SettingsOk { get; set; }
    int BlueGain { get; set; }
    int RedGain { get; set; }
    int GreenGain { get; set; }
    bool WhiteBalanceOnce { get; set; }
    bool GammaEnabled { get; set; }
    double Gamma { get; set; }
    bool AutoGainEnabled { get; set; }
    bool GainBoostEnabled { get; set; }
    int Gain { get; set; }
    void RunCamera();
    void StopCamera();
    void StartAcquisition();
    void StopAcquisition();
    void CloseCamera();
    void CaptureVideo();
    Task<Bitmap> CaptureImage();
    bool IsAssigned { get; set; }
  }
}
