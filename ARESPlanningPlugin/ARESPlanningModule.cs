using AresAdditivePlanningPlugin.PlannerManagers;
using AresAdditivePlanningPlugin.Planners;
using AresAdditivePlanningPlugin.Planners.GradientDescent;
using AresAdditivePlanningPlugin.Planners.GradientDescent.Views;
using AresAdditivePlanningPlugin.Planners.GradientDescent.Views.ViewModels;
using AresAdditivePlanningPlugin.Planners.Views;
using AresAdditivePlanningPlugin.Planners.Views.ViewModels;
using ARESCore.PlanningSupport;
using ARESCore.PluginSupport;
using ARESCore.Registries;
using CommonServiceLocator;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AresAdditivePlanningPlugin
{
  [Module(ModuleName = "AresAdditivePlanningModule", OnDemand = false)]
  public class AresAdditivePlanningModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public AresAdditivePlanningModule(IRegionManager regionManager) : base(regionManager) { }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;

      containerRegistry.RegisterForNavigation<SimplePlannerView, SimplePlannerViewModel>();
      containerRegistry.RegisterForNavigation<GradientDescentPlannerView, GradientDescentPlannerViewModel>();
      containerRegistry.RegisterSingleton<IAresPlanner, SimplePlanner>();
      containerRegistry.RegisterSingleton<IAresPlanner, GradientDescentPlanner>();
    }

    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var reg = ServiceLocator.Current.GetInstance<IAresPlannerManagerRegistry>();
      reg.Add(ServiceLocator.Current.GetInstance<SimplePlannerManager>());
      reg.Add(ServiceLocator.Current.GetInstance<GradientDescentPlannerManager>());
    }
  }
}
