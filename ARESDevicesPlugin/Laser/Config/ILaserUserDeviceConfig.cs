using ARESCore.Configurations;
using ARESCore.UserSession;

namespace ARESDevicesPlugin.Laser.Config
{
  public interface ILaserUserDeviceConfig : IUserDeviceConfig
  {
    StartupStateType StartupType { get; set; }
    double LaserPower { get; set; }
  }
}
