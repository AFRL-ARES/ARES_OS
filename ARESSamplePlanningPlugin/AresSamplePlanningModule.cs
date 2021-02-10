using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.PlanningSupport;
using ARESCore.PluginSupport;
using ARESCore.Registries;
using AresSamplePlanningPlugin.PlannerManagers;
using AresSamplePlanningPlugin.Planners.GradientDescent;
using AresSamplePlanningPlugin.Planners.GradientDescent.Views;
using AresSamplePlanningPlugin.Planners.GradientDescent.Views.ViewModels;
using AresSamplePlanningPlugin.Planners.Simple;
using AresSamplePlanningPlugin.Planners.Simple.Views;
using AresSamplePlanningPlugin.Planners.Simple.Views.ViewModels;
using CommonServiceLocator;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ARESSamplePlanningPlugin
{
  [Module(ModuleName = "AresSamplePlanningModule", OnDemand = false)]
  public class AresSamplePlanningModule : AresModule
  {
    public AresSamplePlanningModule(IRegionManager regionManager) : base(regionManager)
    {
    }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      containerRegistry.RegisterForNavigation<GradientDescentPlannerView, GradientDescentPlannerViewModel>();
      containerRegistry.RegisterForNavigation<SimplePlannerView, SimplePlannerViewModel>();
      containerRegistry.RegisterSingleton<IAresPlanner, GradientDescentPlanner>();
      containerRegistry.RegisterSingleton<IAresPlanner, SimplePlanner>();
    }

    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var reg = ServiceLocator.Current.GetInstance<IAresPlannerManagerRegistry>();
      reg.Add(ServiceLocator.Current.GetInstance<GradientDescentPlannerManager>());
      reg.Add(ServiceLocator.Current.GetInstance<SimplePlannerManager>());
    }
  }
}
