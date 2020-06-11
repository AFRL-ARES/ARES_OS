using DynamicData.Binding;
using ReactiveUI;

namespace ARESCore.Experiment.Results.impl
{
  public class StepExecutionExecutionSummary : ExecutionSummary, IStepExecutionSummary
  {
    private ObservableCollectionExtended<ICommandExecutionSummary> _commandExecutionSummaries = new ObservableCollectionExtended<ICommandExecutionSummary>();
    private string _stepName = string.Empty;

    public string StepName
    {
      get => _stepName;
      set => this.RaiseAndSetIfChanged( ref _stepName, value );
    }

    public ObservableCollectionExtended<ICommandExecutionSummary> CommandExecutionSummaries
    {
      get => _commandExecutionSummaries;
      set => this.RaiseAndSetIfChanged( ref _commandExecutionSummaries, value );
    }
  }
}
