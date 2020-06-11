using DynamicData.Binding;

namespace ARESCore.Experiment.Results
{
  public interface IStepExecutionSummary : IExecutionSummary
  {
    string StepName { get; set; }
    ObservableCollectionExtended<ICommandExecutionSummary> CommandExecutionSummaries { get; set; }
    // I don't think knowing whether or not the step was parallel matters after execution
  }
}
