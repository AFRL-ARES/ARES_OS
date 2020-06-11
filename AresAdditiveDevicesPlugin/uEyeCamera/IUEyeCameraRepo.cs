using System.Drawing;
using System.Threading.Tasks;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.UEyeCamera
{
  public interface IUEyeCameraRepo : IObservableCollection<IUEyeCamera>
  {
    IUEyeCamera GetCamera(int index = -1);
    Task<Bitmap> CaptureAnalysisImage();
  }
}
