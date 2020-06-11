using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ReactiveUI;

namespace ARESCore.PlanningSupport.Impl
{
  internal class PlanResults : BasicReactiveObjectDisposable, IPlanResults
  {
    private IPlannedExperimentBatchInputs _results;

    public IPlannedExperimentBatchInputs Results
    {
      get => _results;
      set => this.RaiseAndSetIfChanged( ref _results, value );
    }
  }
}
