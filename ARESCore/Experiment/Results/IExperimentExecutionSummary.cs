using DynamicData.Binding;

namespace ARESCore.Experiment.Results
{
  public interface IExperimentExecutionSummary : IExecutionSummary
  {
    int ExperimentNumber { get; set; }
    double Result { get; set; }
    object ResultBase { get; set; }
    ObservableCollectionExtended<IStepExecutionSummary> StepExecutionSummaries { get; set; }
  }
}
