using AresCNTDevicesPlugin.Laser.Config;
using AresCNTDevicesPlugin.Laser.UI.Config;
using AresCNTDevicesPlugin.Laser.UI.Control;
using ARESCore.DeviceSupport;
using ARESCore.Experiment;
using ARESCore.PluginSupport;
using ARESCore.Registries;
using ARESDevicesPlugin.Data;
using CommonServiceLocator;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ARESDevicesPlugin
{
  [Module(ModuleName = "ARESDevicesModule", OnDemand = false)]
  public class ARESDevicesModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public ARESDevicesModule(IRegionManager regionManager) : base(regionManager)
    {
    }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;
      RegisterObjects();
      RegisterConfigViews();
      RegisterControlViews();
    }

    private void RegisterConfigViews()
    {
      RegisterAppConfigViews();
      RegisterUserConfigViews();
    }

    private void RegisterAppConfigViews()
    {
      _containerRegistry.RegisterForNavigation<LaserAppConfigView, LaserAppConfigViewModel>();
    }

    private void RegisterUserConfigViews()
    {
      _containerRegistry.RegisterForNavigation<LaserUserConfigView, LaserUserConfigViewModel>();
    }

    private void RegisterControlViews()
    {
      _containerRegistry.RegisterForNavigation<LaserControlView, LaserControlViewViewModel>();
    }


    private void RegisterObjects()
    {
      RegisterUserConfigObjects();
      RegisterAppConfigObjects();
      RegisterConfigManagerObjects();

      _containerRegistry.RegisterSingleton<IDeviceScriptData, FcScriptData>();
    }

    private void RegisterUserConfigObjects()
    {
      _containerRegistry.RegisterSingleton<ILaserUserDeviceConfig, LaserUserDeviceConfig>();
    }

    private void RegisterAppConfigObjects()
    {
      _containerRegistry.RegisterSingleton<ILaserAppConfig, LaserAppConfig>();
    }

    private void RegisterConfigManagerObjects()
    {
      _containerRegistry.RegisterSingleton<ILaserConfigManager, LaserConfigManager>();
    }

    private void RegisterSidebarViews()
    {
      var sidebarRegionName = "SidebarRegion";
    }

    private void RegisterDeviceConfigurationViews()
    {
      RegisterSerialDeviceConfigViews();
    }

    private void RegisterSerialDeviceConfigViews()
    {
      var deviceConfigRegionName = "DeviceConfigRegion";
      _regionManager.RegisterViewWithRegion(deviceConfigRegionName, typeof(LaserAppConfigView));
    }


    private void RegisterConfigManagers(IContainerProvider containerProvider)
    {
      var reg = containerProvider.Resolve<IConfigManagerRegistry>();
      reg.Add(containerProvider.Resolve<ILaserConfigManager>());
    }

    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var devices = ServiceLocator.Current.GetAllInstances<IAresDevice>();

      RegisterConfigManagers(containerProvider);
      RegisterSidebarViews();
      RegisterDeviceConfigurationViews();

      foreach (IAresDevice device in devices)
      {
        device.Init();
        device.RegisterCommands(_registry);
      }
      foreach (IAresDevice device in devices)
      {
        device.Activate();
      }
      var cntScripts = ServiceLocator.Current.GetInstance<IDeviceScriptData>();
      var campaign = ServiceLocator.Current.GetInstance<ICampaign>();
      campaign.ExpScript = cntScripts.ExpScript;
      campaign.InterExpScript = cntScripts.InterExpScript;
      campaign.CampaignCloseScript = cntScripts.CampaignCloseScript;
    }
  }
}