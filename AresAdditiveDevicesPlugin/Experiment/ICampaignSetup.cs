using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Experiment.Impl;
using AresAdditiveDevicesPlugin.PythonStageController;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment
{
  public interface ICampaignSetup
  {
    IExperimentGrid Grid { get; set; }
    IToolpathParameters VarEntries { get; set; }

    ReactiveCommand<Unit, Unit> GenerateGridCommand { get; set; }
    ReactiveCommand<Unit, double> UseCurrentXCommand { get; set; }
    ReactiveCommand<Unit, double> UseCurrentYCommand { get; set; }
    ReactiveCommand<Unit, double> UseCurrentZCommand { get; set; }
    ReactiveCommand<Unit, double> UseCurrentECommand { get; set; }
    ReactiveCommand<Unit, Task> GenerateExtents { get; set; }
    void SetInitialPositionAt(int index);
  }
}
