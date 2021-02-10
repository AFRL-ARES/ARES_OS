using ARESCore.Configurations.impl;
using AresSampleDevicesPlugin.SampleDevice.UI.Config;

namespace AresSampleDevicesPlugin.SampleDevice.Config
{
  public class SampleDeviceConfigManager : ConfigManager, ISampleDeviceConfigManager
  {

    public SampleDeviceConfigManager(ISampleDeviceUserDeviceConfig userConfig, ISampleDeviceAppConfig appConfig, SampleDeviceUserConfigView userConfigView, SampleDeviceAppConfigView appConfigView)
    {
      UserDeviceConfig = userConfig;
      AppConfig = appConfig;
      UserConfigView = userConfigView;
      AppConfigView = appConfigView;
      DeviceName = "Sample Device";

    }
  }
}
