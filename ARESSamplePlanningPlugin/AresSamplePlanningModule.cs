using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.PlanningSupport;
using ARESCore.PluginSupport;
using ARESCore.Registries;
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
  }
}
