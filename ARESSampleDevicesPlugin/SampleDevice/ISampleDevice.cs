using System.Threading.Tasks;
using ARESCore.DeviceSupport;

namespace AresSampleDevicesPlugin.SampleDevice
{
  public interface ISampleDevice : IAresDevice
  {
    Task SetDouble(double value);
    Task SetBool(bool value);
    Task GetDouble();
    Task GetBool();
    double DoubleValue { get; }
    bool BoolValue { get; }
  }
}
