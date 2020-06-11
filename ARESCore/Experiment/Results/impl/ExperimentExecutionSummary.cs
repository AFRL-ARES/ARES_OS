using DynamicData.Binding;
using ReactiveUI;

namespace ARESCore.Experiment.Results.impl
{
  public class ExperimentExecutionSummary : ExecutionSummary, IExperimentExecutionSummary
  {
    private int _experimentNumber;
    private ObservableCollectionExtended<IStepExecutionSummary> _stepExecutionSummaries = new ObservableCollectionExtended<IStepExecutionSummary>();
    private double _result;
    private object _resultBase;

    public int ExperimentNumber
    {
      get => _experimentNumber;
      set => this.RaiseAndSetIfChanged(ref _experimentNumber, value);
    }

    public double Result
    {
      get => _result;
      set => this.RaiseAndSetIfChanged(ref _result, value);
    }

    public object ResultBase
    {
      get => _resultBase;
      set => this.RaiseAndSetIfChanged(ref _resultBase, value);
    }

    public ObservableCollectionExtended<IStepExecutionSummary> StepExecutionSummaries
    {
      get => _stepExecutionSummaries;
      set => this.RaiseAndSetIfChanged(ref _stepExecutionSummaries, value);
    }
  }
}
