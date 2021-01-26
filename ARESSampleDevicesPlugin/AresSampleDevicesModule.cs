using ARESCore.DeviceSupport;
using ARESCore.Experiment;
using ARESCore.PluginSupport;
using ARESCore.Registries;
using AresSampleDevicesPlugin.SampleDevice.Config;
using AresSampleDevicesPlugin.SampleDevice.UI.Config;
using AresSampleDevicesPlugin.SampleDevice.UI.Control;
using CommonServiceLocator;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AresSampleDevicesPlugin
{
  [Module(ModuleName = "AresSampleDevicesModule", OnDemand = false)]
  // ReSharper disable once UnusedMember.Global
  public class AresSampleDevicesModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public AresSampleDevicesModule(IRegionManager regionManager) : base(regionManager)
    {
    }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;
      RegisterObjects();
      RegisterViews();
    }


    private void RegisterViews()
    {

      _containerRegistry.RegisterForNavigation<SampleDeviceAppConfigView, SampleDeviceAppConfigViewModel>();
      _containerRegistry.RegisterForNavigation<SampleDevice.UI.Config.SampleDeviceUserConfigView, SampleDeviceUserConfigViewModel>();
      _containerRegistry.RegisterForNavigation<SampleDeviceControlView, SampleDeviceControlViewModel>();
    }


    private void RegisterObjects()
    {
      _containerRegistry.RegisterSingleton<IDeviceScriptData, SampleScriptData>();
      _containerRegistry.RegisterSingleton<ISampleDeviceUserDeviceConfig, SampleDeviceUserDeviceConfig>();
      _containerRegistry.RegisterSingleton<ISampleDeviceAppConfig, SampleDeviceAppConfig>();
      _containerRegistry.RegisterSingleton<ISampleDeviceConfigManager, SampleDeviceConfigManager>();
    }

    private void RegisterSidebarViews()
    {
      const string sidebarRegionName = "SidebarRegion";
      _regionManager.RegisterViewWithRegion(sidebarRegionName, typeof(SampleDeviceControlView));
    }

    private void RegisterDeviceConfigurationViews()
    {
      RegisterSerialDeviceConfigViews();
    }

    private void RegisterSerialDeviceConfigViews()
    {
      const string deviceConfigRegionName = "DeviceConfigRegion";
      //      var usbConfigRegionName = "UsbDeviceConfigRegion";
      //      var miscConfigRegionName = "MiscConfigRegion";
      _regionManager.RegisterViewWithRegion(deviceConfigRegionName, typeof(SampleDeviceAppConfigView));
    }

    private void RegisterConfigManagers(IContainerProvider containerProvider)
    {
      var reg = containerProvider.Resolve<IConfigManagerRegistry>();
      reg.Add(containerProvider.Resolve<ISampleDeviceConfigManager>());
    }

    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var devices = ServiceLocator.Current.GetAllInstances<IAresDevice>();
      RegisterConfigManagers(containerProvider);
      RegisterSidebarViews();
      //      var menuRegionName = "MenuRegion";
      RegisterDeviceConfigurationViews();

      foreach (var device in devices)
      {
        device.Init();
        device.RegisterCommands(_registry);
      }
      var cntScripts = ServiceLocator.Current.GetInstance<IDeviceScriptData>();
      var campaign = ServiceLocator.Current.GetInstance<ICampaign>();
      campaign.ExpScript = cntScripts.ExpScript;
      campaign.InterExpScript = cntScripts.InterExpScript;
      campaign.CampaignCloseScript = cntScripts.CampaignCloseScript;
    }
  }
}
