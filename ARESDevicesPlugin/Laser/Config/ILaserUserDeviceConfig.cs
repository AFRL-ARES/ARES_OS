using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Configurations;
using ARESCore.UserSession;

namespace AresCNTDevicesPlugin.Laser.Config
{
  public interface ILaserUserDeviceConfig : IUserDeviceConfig
  {
    StartupStateType StartupType { get; set; }
    double LaserPower { get; set; }
  }
}
