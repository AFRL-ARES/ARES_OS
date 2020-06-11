using System.Threading.Tasks;

namespace ARESCore.Experiment.Scripting.Commands
{
  public class DelayCommand : AresScriptCommand<double>
  {
    public override string ScriptName { get; } = "DELAY";
    public override int ArgCount { get; } = 1;

    public override bool ArgCountEnforced { get; } = true;
    public override string HelpString { get; } = "Usage: Supply a value denoting the amount of milliseconds to wait before continuing.\n" +
                                                 "Ex. DELAY 250 (Delays 250 seconds before this step can continue)";

    public override bool Validate(string[] args)
    {
      if (args.Length != 1)
        return false;
      if (double.TryParse(args[0], out var result))
        return result >= 0;
      return false;
    }

    public override async Task Execute(string[] args)
    {
      await Task.Delay(int.Parse(args[0]));
    }
  }
}
