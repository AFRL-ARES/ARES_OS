using System;
using System.Collections.Generic;
using System.Linq;
using AresCNTDevicesPlugin.Laser.UI.Config;
using ARESCore.Configurations.impl;

namespace AresCNTDevicesPlugin.Laser.Config
{
  public class LaserConfigManager: ConfigManager,ILaserConfigManager
  {

    public LaserConfigManager(ILaserUserDeviceConfig userConfig, ILaserAppConfig appConfig, LaserUserConfigView userConfigView, LaserAppConfigView appConfigView)
    {
      UserDeviceConfig = userConfig;
      AppConfig = appConfig;
      UserConfigView = userConfigView;
      AppConfigView = appConfigView;
      DeviceName = "Laser (Verdi V6)";

    }
  }
}
