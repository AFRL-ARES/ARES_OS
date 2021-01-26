using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.impl;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace ARESCore.PlanningSupport.Impl
{
  public class ManualPlanningViewModel : ReactiveSubscriber
  {
    private readonly IDialogCoordinator _coordinator;
    private string _filePathText;

    public ManualPlanningViewModel(IDialogCoordinator coordinator)
    {
      _coordinator = coordinator;
    }

    public PlannedExperimentBatchInputs BatchInputs { get; set; } = new PlannedExperimentBatchInputs();

    public string FilePathText
    {
      get => _filePathText;
      set
      {
        this.RaiseAndSetIfChanged( ref _filePathText, value );
        CheckPath();
      }
    }

    private async void CheckPath()
    {
      if ( !File.Exists( FilePathText ) )
        return;
      //FIXME: rebuild Custom datafile column chooser, add to this
      // Get the suspected order of the columns in the csv
      //var expDataDesc = new List<string>();
      // var cdfColChooser = new CustomDataFileColumnChooser();
      //cdfColChooser.ShowDefault();
      //if (SelectedBatchType == BatchType.CNT_GROWTH )
      //    cdfColChooser.ShowCNTGrowth();
      try
      { BatchInputs.LoadInputsFromFile( FilePathText ); }
      catch ( Exception ex )
      {
        await _coordinator.ShowMessageAsync( this, "Load Error", "Error parsing experiments from file!\n" + ex.Message);
      }
    }
  }
}