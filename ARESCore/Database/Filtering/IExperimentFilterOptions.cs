using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.AnalysisSupport;
using ARESCore.PlanningSupport;

namespace ARESCore.Database.Filtering
{
  public interface IExperimentFilterOptions: IFilterOptions
  {
     bool StrictFiltering { get; set; }
     bool FilterBatchType { get; set; }
     bool FilterProjectDescription { get; set; }
     bool FilterExperimentDate { get; set; }
     bool FilterPlannerType { get; set; }


     IAresAnalyzer BatchTypeFilter { get; set; }
     Type PostProcessingDataType { get; set; } 
     List<string> Projects { get; set; }
     DateTime FromDate { get; set; }
     DateTime ToDate { get; set; }
     IAresPlannerManager Planner { get; set; }

  }
}
