namespace ARESCore.Experiment.Results
{
  public interface ICommandExecutionSummary : IExecutionSummary
  {
    string Command { get; set; }
    string Value { get; set; }
  }
}
