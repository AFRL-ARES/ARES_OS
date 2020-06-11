using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARESCore.Experiment.Scripting.Commands
{
  public class WaitCommand : AresScriptCommand<double>
  {
    public override string ScriptName { get; } = "WAIT";
    public override int ArgCount { get; } = 1;

    public override bool ArgCountEnforced { get; } = true;
    public override string HelpString { get; } = "Usage: Supply a value denoting the amount of time to wait before continuing.\n" +
    "Ex. WAIT 10 (Waits 10 seconds before this step can continue)";

    public override bool Validate( string[] args )
    {
      if ( args.Length != 1 )
        return false;
      double result;
      if ( double.TryParse( args[0], out result ) )
        return true;
      return false;
    }

    public override async Task Execute(string[] args)
    {
      await Task.Delay( int.Parse( args[0]) * 1000 );
    }
  }
}