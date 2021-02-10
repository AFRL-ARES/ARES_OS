using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ARESCore.DisposePatternHelpers;
using ARESCore.PlanningSupport;
using MoreLinq;
using ReactiveUI;

namespace ARESCore.Experiment.UI.ViewModels
{
  public class PlanResultsViewModel : ReactiveSubscriber
  {
    private readonly IPlanResults _planResults;
    private bool _plansVisible;

    public PlanResultsViewModel(IPlanResults planResults)
    {
      _planResults = planResults;
      RowEntries = new ObservableCollection<ObservableCollection<double>>();
      _planResults.WhenAnyValue(t => t.Results).Subscribe(t => Update());
    }

    private void Update()
    {
      PlannerColumns.Clear();
      if (_planResults.Results == null)
        return;
      var replacedstrs = new List<string>();
      _planResults.Results.PlannedInputs.FirstOrDefault()?.Inputs.Select(param => param.Name)
        .ForEach(input => replacedstrs.Add(input.Replace("_", "__")));
      PlannerColumns.AddRange(replacedstrs);
      Application.Current.Dispatcher.BeginInvoke(new Action(() =>
    {
      RowEntries.Clear();
      for (var index = 0; index < _planResults.Results.PlannedInputs.Count; index++)
      {
        var row = _planResults.Results.PlannedInputs[index].Inputs.Select(param => param.Value);
        var obrow = new ObservableCollection<double>();
        obrow.AddRange(row);
        RowEntries.Add(obrow);
      }
    }));
      PlansVisible = true;
    }


    public ObservableCollection<ObservableCollection<double>> RowEntries { get; set; }

    public ObservableCollection<string> PlannerColumns { get; set; } = new ObservableCollection<string>();


    public bool PlansVisible
    {
      get => _plansVisible;
      set => this.RaiseAndSetIfChanged(ref _plansVisible, value);
    }
  }
}
