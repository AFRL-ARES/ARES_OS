using System;
using System.Collections.Generic;
using ARESCore.AnalysisSupport;
using ARESCore.PlanningSupport;
using DynamicData.Binding;

namespace ARESCore.Registries
{
  public interface IAresPlannerRegistry: IObservableCollection<IAresPlanner>
  {
  }
}
