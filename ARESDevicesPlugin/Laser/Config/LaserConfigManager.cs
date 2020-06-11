using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Configurations.impl;
using ARESDevicesPlugin.Laser.UI.Config;

namespace ARESDevicesPlugin.Laser.Config
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
