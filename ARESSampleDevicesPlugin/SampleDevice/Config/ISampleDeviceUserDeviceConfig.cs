using ARESCore.Configurations;
using ARESCore.UserSession;

namespace AresSampleDevicesPlugin.SampleDevice.Config
{
  public interface ISampleDeviceUserDeviceConfig : IUserDeviceConfig
  {
    StartupStateType StartupType { get; set; }
    double DoubleValue { get; set; }
  }
}
