using System;
using System.Collections.Generic;
using ARESCore.AnalysisSupport;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using ReactiveUI;

namespace ARESCore.Database.Filtering.Impl
{

  public class ExperimentFilterOptions : BasicReactiveObjectDisposable, IExperimentFilterOptions
  {
    private DateTime _fromDate;
    private DateTime _toDate = DateTime.Now;
    private Type _postProcessingDataType;
    private bool _filterPlannerType;
    private bool _filterExperimentDate;
    private bool _filterProjectDescription;
    private bool _filterBatchType;
    private bool _strictFiltering;
    private IAresPlannerManager _planner;

    public ExperimentFilterOptions()
    {
      //      FromDate = DateTime.Now.Subtract( TimeSpan.FromDays( 365 ) );
      FromDate = DateTime.Parse( "10/1/2018" );
    }

    public bool StrictFiltering
    {
      get => _strictFiltering;
      set => this.RaiseAndSetIfChanged( ref _strictFiltering, value );
    }

    public bool FilterBatchType
    {
      get => _filterBatchType;
      set => this.RaiseAndSetIfChanged( ref _filterBatchType, value );
    }

    public bool FilterProjectDescription
    {
      get => _filterProjectDescription;
      set => this.RaiseAndSetIfChanged( ref _filterProjectDescription, value );
    }

    public bool FilterExperimentDate
    {
      get => _filterExperimentDate;
      set => this.RaiseAndSetIfChanged( ref _filterExperimentDate, value );
    }

    public bool FilterPlannerType
    {
      get => _filterPlannerType;
      set => this.RaiseAndSetIfChanged( ref _filterPlannerType, value );
    }


    public IAresAnalyzer BatchTypeFilter { get; set; }

    public Type PostProcessingDataType
    {
      get => _postProcessingDataType;
      set => this.RaiseAndSetIfChanged( ref _postProcessingDataType, value );
    }

    public List<string> Projects { get; set; } = new List<string>();

    public DateTime FromDate
    {
      get => _fromDate;
      set => this.RaiseAndSetIfChanged( ref _fromDate, value );
    }

    public DateTime ToDate
    {
      get => _toDate;
      set => this.RaiseAndSetIfChanged( ref _toDate, value );
    }

    public IAresPlannerManager Planner
    {
      get => _planner;
      set => this.RaiseAndSetIfChanged( ref _planner, value );
    }


    // Machine State filters. TODO FIXME: move out
    public bool FilterChipDescriptions { get; set; } = false;
    public List<Guid> Chips { get; set; } = new List<Guid>();






  }
}
