using ARESCore.PlanningSupport;
using ARESCore.PluginSupport;
using ARESCore.Registries;
using AresPlanningPlugin.PlannerManagers;
using AresPlanningPlugin.Planners;
using AresPlanningPlugin.Planners.Views;
using CommonServiceLocator;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AresPlanningPlugin
{
  [Module(ModuleName = "AresPlanningModule", OnDemand = false)]
  public class AresPlanningModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public AresPlanningModule(IRegionManager regionManager) : base(regionManager) { }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;

      containerRegistry.RegisterForNavigation<SimplePlannerView, Planners.Views.ViewModels.SimplePlannerViewModel>();
      containerRegistry.RegisterSingleton<IAresPlanner, SimplePlanner>();
    }

    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var reg = ServiceLocator.Current.GetInstance<IAresPlannerManagerRegistry>();
      reg.Add(ServiceLocator.Current.GetInstance<SimplePlannerManager>());
    }
  }
}
