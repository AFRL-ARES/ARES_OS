using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.AnalysisSupport;
using ARESCore.PlanningSupport;

namespace ARESCore.Database.Filtering.Impl
{

  public class FilterOptions
  {
    public bool StrictFiltering { get; set; } = false;
    public bool FilterBatchType { get; set; } = false;
    public bool FilterProjectDescription { get; set; } = false;
    public bool FilterExperimentDate { get; set; } = false;
    public bool FilterPlannerType { get; set; } = false;


    public IAresAnalyzer BatchTypeFilter { get; set; }
    public Type PostProcessingDataType { get; set; } = null;
    public List<string> Projects { get; set; } = new List<string>();
    public DateTime FromDate { get; set; } = default(DateTime);
    public DateTime ToDate { get; set; } = default(DateTime);


    // Machine State filters. TODO FIXME: move out
    public bool FilterChipDescriptions { get; set; } = false;

    public List<Guid> Chips { get; set; } = new List<Guid>();



    // Data filters. TODO FIXME: Move out
    public bool FilterCNTGBandAvg { get; set; } = false; // Seems like CNTGrowthPostProcess filter, but really resides in AOI!

    public double GBandAvgMinimum { get; set; } = 5000;
    public bool FilterCNTR2Minimum { get; set; } = false;
    public double R2Minimum { get; set; } = 0.5;
    public IAresPlannerManager Planner { get; set; }
  }
}
