using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ARESCore.Commands;
using ARESCore.ErrorSupport.Impl;
using ARESCore.ErrorSupport.Impl.RetryInfos;
using ARESCore.Experiment.Results;
using ARESCore.Extensions;
using ARESCore.Registries;
using ARESCore.UserSession;
using DynamicData.Binding;
using NationalInstruments.Restricted;
using Ninject;

namespace ARESCore.Experiment.Scripting.Commands
{
  public class StepStartSequentialCommand : AresScriptCommand<string>
  {
    // Making this a list still makes it surprisingly easier to manage than just a single nullable retryInfo
    private readonly List<CommandRetryInfo> _retryInfos = new List<CommandRetryInfo>();

    private readonly ICampaignExecutionSummary _campaignExecutionSummary;
    private readonly ICurrentConfig _currentConfig;
    private readonly IAresCommandRegistry _commandRegistry;
    public override string ScriptName { get; } = "STEP_SEQ";
    public override string CloserCmd { get; } = "STEP_END";

    public override int ArgCount { get; } = 1;

    public override string HelpString { get; } = "Denotes the start of a step.\n" +
                                                 "Usage: A step name may be supplied after the command.\n" +
                                                 "Ex. STEP_SEQ This is the step name\n" +
                                                 "**All commands within a step are completed sequentially before the next step proceeds**";

    private bool _terminated;
    public StepStartSequentialCommand( ICampaignExecutionSummary campaignExecutionSummary, ICurrentConfig currentConfig, IAresCommandRegistry commandRegistry )

    {
      _campaignExecutionSummary = campaignExecutionSummary;
      _currentConfig = currentConfig;
      _commandRegistry = commandRegistry;
    }

    public override bool Validate( string[] args )
    {
      return args.Length > 0;
    }

    public override async Task Execute( string[] lines )
    {
      _terminated = false;
      var currExp = _campaignExecutionSummary.ExperimentExecutionSummaries.Last();
      var currStep = currExp.StepExecutionSummaries.Last();
      currStep.StepName = lines[0].Substring( ScriptName.Length );
      var innerCommands = new List<string>( lines );
      innerCommands.RemoveAt( 0 );
      innerCommands.Remove( innerCommands.Last() );
      currStep.ExecutionDuration = TimeSpan.Zero;
      // start timer
      var prec = TimeSpan.FromSeconds( _currentConfig.TimerPrecision );
      var timerObservable = Observable.Interval( prec )
        .Subscribe( _ => currStep.ExecutionDuration = currStep.ExecutionDuration.Add( prec ) );
      for ( var index = 0; index < innerCommands.Count; index++ )
      {
        if ( _terminated )
        {
          return;
        }
        var commandStr = innerCommands[index];
        var lineTokens = commandStr.Split( ' ' );
        var cmd = lineTokens[0];
        var command = _commandRegistry.FirstOrDefault( c => c.ScriptName != null && c.ScriptName.Equals( cmd ) );
        List<string> revisedTokens = new List<string>();
        for ( int k = 1; k < lineTokens.Length; k++ )
        {
          revisedTokens.Add( lineTokens[k] );
        }
        var cmdResult = AresKernel._kernel.Get<ICommandExecutionSummary>();
        cmdResult.Command = cmd;
        cmdResult.Status = ExecutionStatus.EXECUTING;
        cmdResult.Value = string.Join( " ", revisedTokens );
        currStep.CommandExecutionSummaries.Add( cmdResult );
        try
        {

          await RunTask( command, revisedTokens, cmdResult );
          do
            await Task.Delay( 500 );
          while ( _retryInfos.Any() );
        }
        catch ( Exception e )
        {
        }
        finally
        {
          timerObservable.Dispose();
        }
      }
      if ( _retryInfos.IsEmpty() )
      {
        IsComplete = true;
      }

    }

    private async Task RunTask( IAresCommand command, List<string> revisedTokens, ICommandExecutionSummary cmdExecutionSummary )
    {
      var cmdErrorSub = command.WhenPropertyChanged( cmd => cmd.Error, false ).Take( 1 ).Subscribe( error => OnFail( command, revisedTokens, cmdExecutionSummary ) );
      var prec = TimeSpan.FromSeconds( _currentConfig.TimerPrecision );
      cmdExecutionSummary.ExecutionDuration = TimeSpan.Zero;
      var taskTimer = Observable.Interval( prec ).Subscribe( r => cmdExecutionSummary.ExecutionDuration = cmdExecutionSummary.ExecutionDuration.Add( prec ) );
      Random rand = new Random();
      await Task.Delay( TimeSpan.FromSeconds( rand.Next() % 3 ) );
      try
      {
        await command.Execute( revisedTokens.ToArray() );
        cmdExecutionSummary.Status = ExecutionStatus.DONE;
      }
      catch ( Exception e )
      {
        cmdExecutionSummary.Status = ExecutionStatus.ERROR;
      }
      finally
      {
        taskTimer.Dispose();
        cmdErrorSub.Dispose();
      }
    }

    public override async Task Handle( ErrorResponse response )
    {
      await base.Handle( response );
      if ( !_retryInfos.Any() )
      {
        IsComplete = _retryInfos.IsEmpty();
      }
    }

    protected override async Task HandleRetry()
    {
      try
      {
        var retryInfo = _retryInfos.FirstOrDefault();
        await RunTask( retryInfo.Command, retryInfo.RevisedTokens, retryInfo.CommandExecutionSummary );
      }
      catch ( Exception e )
      {

      }
      _retryInfos.RemoveAt( 0 );
    }

    protected override Task HandleIgnoreAndContinue()
    {
      _retryInfos.RemoveAt( 0 );
      return Task.CompletedTask;
    }

    protected override Task HandleStop()
    {
      _retryInfos.Clear();
      _terminated = true;
      Error = new AresError { Text = "Script Handling Sequential Step" };
      IsComplete = true;
      return Task.CompletedTask;
    }

    private void OnFail( IAresCommand command, List<string> revisedTokens, ICommandExecutionSummary cmdExecutionSummary )
    {
      var retryInfo = new CommandRetryInfo { Command = command, RevisedTokens = revisedTokens, CommandExecutionSummary = cmdExecutionSummary };
      _retryInfos.Add( retryInfo );
      CreateAndAddErrorBundle( this, command );
    }
  }
}
