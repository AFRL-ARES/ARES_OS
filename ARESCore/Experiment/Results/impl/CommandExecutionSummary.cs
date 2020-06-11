using ReactiveUI;

namespace ARESCore.Experiment.Results.impl
{
  public class CommandExecutionSummary : ExecutionSummary, ICommandExecutionSummary
  {
    private string _value = string.Empty;
    private string _command = string.Empty;

    public string Command
    {
      get => _command;
      set => this.RaiseAndSetIfChanged( ref _command, value );
    }

    public string Value
    {
      get => _value;
      set => this.RaiseAndSetIfChanged( ref _value, value );
    }
  }
}
