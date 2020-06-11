using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARESCore.Experiment.Scripting.Commands
{
  public class StepEndCommand : AresScriptCommand<string>
  {
    public override string ScriptName { get; } = "STEP_END";
    public override int ArgCount { get; } = 0;
    public override string HelpString { get; } = "Denotes the end of a step.";

    public override bool Validate( string[] args )
    {
      if ( args.Length > 0 )
        return false;
      return true;
    }

    public override Task Execute(string[] lines)
    {
      // do nothing?
      return Task.FromResult( 0 );
    }
  }
}
