using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Windows.Controls;
using ARESCore.AnalysisSupport;
using ARESCore.Database.ViewModels;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using ARESCore.PlanningSupport.Impl;
using ARESCore.Registries;
using DynamicData;
using DynamicData.Binding;
using NationalInstruments.Restricted;
using Ninject;
using ReactiveUI;

namespace ARESCore.Experiment.UI.ViewModels
{
  public class BatchInputDataViewModel : BasicReactiveObjectDisposable
  {
    private string _selectedExperimentType;
    private readonly IAresAnalyzerRegistry _registry;
    private string _selectedInputSource;
    private bool _dbGenEnabled;
    private IAresAnalyzer _selectedExperiment;
    private IAresPlannerManager _selectedPlannerManager;
    private readonly IExperimentBatch _experimentBatch;
    private readonly ISelectedPlannersRepository _planners;
    private bool _planningEnabled;
    private double _planTargetValue;
    private readonly Dictionary<IAresPlanner, IDisposable> _plannableSubs = new Dictionary<IAresPlanner, IDisposable>();
    private readonly Dictionary<IAresPlanner, bool> _plannerValidations = new Dictionary<IAresPlanner, bool>();

    public BatchInputDataViewModel()
    {
      _registry = AresKernel._kernel.Get<IAresAnalyzerRegistry>();
      _experimentBatch = AresKernel._kernel.Get<IExperimentBatch>();
      _planners = AresKernel._kernel.Get<ISelectedPlannersRepository>();

      DbGenEnabled = false;
      foreach (var aresAnalyzer in _registry)
      {
        ExperimentTypes.Add(aresAnalyzer.AnalyzerName);
      }
      if (_registry.Count > 0)
      {
        SelectedExperimentType = _registry.First().AnalyzerName;
      }
      _registry.CollectionChanged += RegistryCollectionChanged;
      _planners.CollectionChanged += PlannerSelectionChanged;


      NewInputDataSourceCommand = ReactiveCommand.Create<Unit, Unit>(u => CreateSelectedTile());
      DoPlanningCommand = ReactiveCommand.Create<Unit, Unit>(u => DoPlanning());
      ShiftUpItem = ReactiveCommand.Create<ContentControl, Unit>(c => ShiftUp(c));
      ShiftDownItem = ReactiveCommand.Create<ContentControl, Unit>(c => ShiftDown(c));
      CloseItem = ReactiveCommand.Create<ContentControl, Unit>(c => Close(c));
    }

    private void PlannerSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      var newPlanners = _planners.Where(plannerManager => !_plannableSubs.ContainsKey(plannerManager.Planner)).Select(plannerManager => plannerManager.Planner).ToArray();
      var disposablePlanners = _plannableSubs.Where(plannerSubPair => !_planners.Select(plannerManager => plannerManager.Planner).Contains(plannerSubPair.Key)).ToArray();

      disposablePlanners.ForEach(plannerSubPair =>
     {
       _plannableSubs[plannerSubPair.Key].Dispose();
       _plannableSubs.Remove(plannerSubPair.Key);
       _plannerValidations.Remove(plannerSubPair.Key);
     });

      newPlanners.ForEach(planner =>
     {
       _plannerValidations.Add(planner, planner.CanPlan);
       var enabledWatcher = planner.WhenPropertyChanged(p => p.CanPlan)
         .Subscribe(canPlan =>
         {
           _plannerValidations[canPlan.Sender] = canPlan.Value;
           PlanningEnabled = _plannerValidations.Any() && _plannerValidations.All(plannerPlanUglyVarName => plannerPlanUglyVarName.Value);
         });
       _plannableSubs.Add(planner, enabledWatcher);
     });
    }

    private Unit DoPlanning()
    {
      for (int i = _planners.Count - 1; i >= 0; i--)
      {
        _planners[i].NumExpsToPlan = 1;
        var plans = _planners[i].DoPlanning();
        AresKernel._kernel.Get<IPlanResults>().Results = plans?.Result;
      }
      return new Unit();
    }

    private Unit ShiftUp(ContentControl contentControl)
    {
      var idx = 0;
      foreach (var plannerTile in PlannerTiles)
      {
        if (!plannerTile.Equals(contentControl))
        {
          idx++;
        }
        else
        {
          break;
        }
      }
      if (idx != 0)
      {
        var currplanner = _planners[idx];
        _planners.RemoveAt(idx);
        _planners.Insert(idx - 1, currplanner);
        PlannerTiles.Move(idx, idx - 1);
      }
      return new Unit();
    }

    private Unit ShiftDown(ContentControl contentControl)
    {
      var idx = 0;
      foreach (var plannerTile in PlannerTiles)
      {
        if (!plannerTile.Equals(contentControl))
        {
          idx++;
        }
        else
        {
          break;
        }
      }
      if (idx != PlannerTiles.Count - 1)
      {
        var currplanner = _planners[idx];
        _planners.RemoveAt(idx);
        _planners.Insert(idx + 1, currplanner);
        PlannerTiles.Move(idx, idx + 1);
      }
      return new Unit();
    }

    private Unit Close(ContentControl contentControl)
    {
      for (var index = 0; index < PlannerTiles.Count; index++)
      {
        var plannerTile = PlannerTiles[index];
        if (plannerTile.Equals(contentControl))
        {
          _planners.RemoveAt(index);
          PlannerTiles.Remove(plannerTile);
          if (PlannerTiles.Count == 0)
            PlanningEnabled = false;
          return new Unit();
        }
      }

      return new Unit();
    }

    private Unit CreateSelectedTile()
    {
      var planReg = AresKernel._kernel.Get<IAresPlannerManagerRegistry>();
      var planner = planReg.FirstOrDefault(p => p.PlannerName.Equals(SelectedInputSource));
      if (planner.PlannerTile == null)
        return new Unit();
      if (!PlannerTiles.Contains(planner.PlannerTile))
      {
        PlannerTiles.Add(planner.PlannerTile);
        _planners.Add(planner);
      }
      return new Unit();
    }

    private void RegistryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        ExperimentTypes.Add(_registry.Last().AnalyzerName);
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        var itemToRemove = ExperimentTypes.FirstOrDefault(n => (sender as IAresAnalyzer).AnalyzerName.Equals(n));
        if (itemToRemove != null)
          ExperimentTypes.Remove(itemToRemove);
      }
    }

    public ObservableCollection<string> ExperimentTypes { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<string> PlannerOptions { get; set; } = new ObservableCollection<string>();

    public string SelectedExperimentType
    {
      get => _selectedExperimentType;
      set
      {
        this.RaiseAndSetIfChanged(ref _selectedExperimentType, value);
        SelectedExperiment = _registry.FirstOrDefault(e => e.AnalyzerName.Equals(SelectedExperimentType));
        _experimentBatch.Analyzer = SelectedExperiment;
        GetPlannerOptions();
      }
    }

    public string SelectedInputSource
    {
      get => _selectedInputSource;
      set
      {
        this.RaiseAndSetIfChanged(ref _selectedInputSource, value);
        var planReg = AresKernel._kernel.Get<IAresPlannerManagerRegistry>();
        if (value == null || value.Equals("None", StringComparison.CurrentCultureIgnoreCase))
        {
          _selectedPlannerManager = null;
        }
        else
        {
          _selectedPlannerManager = planReg.FirstOrDefault(p => _selectedInputSource.Equals(p.PlannerName));
        }
        _experimentBatch.PlannerManager = _selectedPlannerManager;

      }
    }

    public ObservableCollection<UserControl> PlannerTiles { get; set; } = new ObservableCollection<UserControl>();

    public ReactiveCommand<Unit, Unit> NewInputDataSourceCommand { get; set; }

    public ReactiveCommand<Unit, Unit> DoPlanningCommand { get; set; }

    public bool DbGenEnabled
    {
      get => _dbGenEnabled;
      set => this.RaiseAndSetIfChanged(ref _dbGenEnabled, value);
    }

    public IAresAnalyzer SelectedExperiment
    {
      get => _selectedExperiment;
      set
      {
        this.RaiseAndSetIfChanged(ref _selectedExperiment, value);
        if (value != null)
        {
          value.IsSelected = true;
        }
        _registry.Where(analyzer => analyzer != value).ForEach(analyzer => analyzer.IsSelected = false);
      }
    }

    private void GetPlannerOptions()
    {
      PlannerOptions.Clear();


      var planReg = AresKernel._kernel.Get<IAresPlannerManagerRegistry>();
      foreach (var planner in planReg)
      {
        if (planner is ManualPlannerManager && SelectedExperiment is CustomExperimentAnalysis)
        {
          PlannerOptions.Add(planner.PlannerName); // custom/manual is only match
          DbGenEnabled = false;
          return;
        }
          PlannerOptions.Add(planner.PlannerName); // let's assume everyone else matches
          DbGenEnabled = true;
      }
      if (PlannerOptions.Count > 0)
      {
        SelectedInputSource = PlannerOptions[0];
      }
    }

    public override void Dispose()
    {
      _registry.CollectionChanged -= RegistryCollectionChanged;
      _planners.CollectionChanged -= PlannerSelectionChanged;
    }

    public void UpdateDbOptions(DatabaseViewerViewModel dbViewerVm)
    {
      // FilteredDatabase = new ObservableCollection<ExperimentEntity>(dbViewerVm.FilteredDatabase);
      // SharedPlannerFilterOptions = dbViewerVm.FilterOptions;
    }

    public ReactiveCommand<ContentControl, Unit> ShiftUpItem { get; set; }
    public ReactiveCommand<ContentControl, Unit> ShiftDownItem { get; set; }
    public ReactiveCommand<ContentControl, Unit> CloseItem { get; set; }

    public bool PlanningEnabled
    {
      get => _planningEnabled;
      set => this.RaiseAndSetIfChanged(ref _planningEnabled, value);
    }

    public double PlanTargetValue
    {
      get => _planTargetValue;
      set => this.RaiseAndSetIfChanged(ref _planTargetValue, value);
    }
  }
}