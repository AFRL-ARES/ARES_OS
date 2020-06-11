using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Controls;
using ARESCore.Configurations;
using ARESCore.Database.Filtering;
using ARESCore.Database.Tables;
using ARESCore.Database.Views;
using ARESCore.DisposePatternHelpers;
using ARESCore.Registries;
using Ninject;
using ReactiveUI;

namespace ARESCore.Database.ViewModels
{
  /// <summary>
  /// DatabaseViewer
  /// </summary>
  public class FilterTabViewModel : BasicReactiveObjectDisposable
  {
    private IDbFilterManager _filter;
    private IExperimentFilterOptions _experimentFilterOptions;
    private IApplicationConfiguration _appConfig;
    private UserControl _dataDbFilter;
    private UserControl _machineDbFilter;
    private UserControl _analysisTypeDbFilter;
    private string _selectedBatchType;
    private string _selectedPlanner;
    private bool _workingStatus;

    public FilterTabViewModel( IApplicationConfiguration appConfig, IExperimentFilterOptions experimentFilterOptions )
    {
      FilterOptions = experimentFilterOptions;
      AppConfig = appConfig;
      PerformFilteringCommand = ReactiveCommand.Create<Unit>( u => PerformFiltering() );
      IAnalysisUIs analysisUIs = null;
      analysisUIs = AresKernel._kernel.TryGetAndThrowOnInvalidBinding<IAnalysisUIs>();
      DataDbFilter = analysisUIs?.DbDocFilterView;
      MachineStateDbFilter = AresKernel._kernel.TryGetAndThrowOnInvalidBinding<IMachineStateUI>()?.MachineStateFilterView;
      var planners = AresKernel._kernel.Get<IAresPlannerManagerRegistry>();
      foreach ( var planner in planners )
      {
        Planners.Add( planner.PlannerName );
      }
      var analyzers = AresKernel._kernel.Get<IAresAnalyzerRegistry>();
      foreach ( var analyzer in analyzers )
      {
        BatchTypes.Add( analyzer.AnalyzerName );
      }
    }

    private void PerformFiltering()
    {
      if ( _filter == null )
      {
        _filter = AresKernel._kernel.Get<IDbFilterManager>();
        _filter.WhenAnyValue( x => x.LastFilterResult ).Subscribe( x => UpdateFilter() );
      }

      var analyzers = AresKernel._kernel.Get<IAresAnalyzerRegistry>();
      var analyzer = analyzers.FirstOrDefault( a => a.AnalyzerName.Equals( SelectedBatchType ) );
      FilterOptions.BatchTypeFilter = analyzer;
      FilterOptions.PostProcessingDataType = analyzer?.GetType();
      var planners = AresKernel._kernel.Get<IAresPlannerManagerRegistry>();
      var planner = planners.FirstOrDefault( p => p.PlannerName.Equals( SelectedPlannerType ) );
      FilterOptions.Planner = planner;
      if ( SelectedProjects != null )
      {
        var projs = new List<string>();
        foreach ( var projectInfo in SelectedProjects )
        {
          projs.Add( projectInfo.Description );
        }
        FilterOptions.Projects = projs;
      }
      WorkingStatus = true;
      _filter.DoFilter();
    }

    private void UpdateFilter()
    {
      if ( _filter.LastFilterResult != null )
      {
        FilteredDatabase = ( _filter.LastFilterResult as IEnumerable<ExperimentEntity> ).ToList();
      }
      WorkingStatus = false;
    }


    public bool WorkingStatus
    {
      get => _workingStatus;
      set => this.RaiseAndSetIfChanged( ref _workingStatus, value );
    }

    public string SelectedPlannerType
    {
      get => _selectedPlanner;
      set => this.RaiseAndSetIfChanged( ref _selectedPlanner, value );
    }

    public List<ExperimentEntity> FilteredDatabase { get; set; } = new List<ExperimentEntity>();

    public UserControl DataDbFilter
    {
      get => _dataDbFilter;
      set => this.RaiseAndSetIfChanged( ref _dataDbFilter, value );
    }

    public ObservableCollection<string> Planners { get; set; } = new ObservableCollection<string>();

    public ObservableCollection<string> MachineStates { get; set; } = new ObservableCollection<string>();

    public UserControl MachineStateDbFilter
    {
      get => _machineDbFilter;
      set => this.RaiseAndSetIfChanged( ref _machineDbFilter, value );
    }

    public ObservableCollection<string> BatchTypes { get; set; } = new ObservableCollection<string>();

    public UserControl AnalysisTypeDbFilter
    {
      get => _analysisTypeDbFilter;
      set => this.RaiseAndSetIfChanged( ref _analysisTypeDbFilter, value );
    }

    public string SelectedBatchType
    {
      get => _selectedBatchType;
      set
      {
        this.RaiseAndSetIfChanged( ref _selectedBatchType, value );
        AnalysisTypeDbFilter = AresKernel._kernel.Get<IAresAnalyzerRegistry>().FirstOrDefault( a => a.AnalyzerName.Equals( SelectedBatchType ) ).AnalysisDbFilter;
      }
    }

    public IApplicationConfiguration AppConfig
    {
      get => _appConfig;
      set => this.RaiseAndSetIfChanged( ref _appConfig, value );
    }

    public ReactiveCommand<Unit, Unit> PerformFilteringCommand { get; set; }

    public IExperimentFilterOptions FilterOptions
    {
      get => _experimentFilterOptions;
      set => this.RaiseAndSetIfChanged( ref _experimentFilterOptions, value );
    }

    public List<IProjectInfo> SelectedProjects { get; set; }
  }
}