using ARESCore.Commands;
using ARESCore.ErrorSupport.Impl;
using ARESCore.ErrorSupport.Impl.RetryInfos;
using ARESCore.Experiment.Results;
using ARESCore.Extensions;
using ARESCore.Registries;
using ARESCore.UserSession;
using DynamicData.Binding;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ARESCore.Experiment.Scripting.Commands
{
  public class StepStartParallelCommand : AresScriptCommand<string>
  {
    private readonly List<CommandRetryInfo> _retryInfos = new List<CommandRetryInfo>();
    private readonly ICurrentConfig _currentConfig;
    private readonly ICampaignExecutionSummary _campaignExecutionSummary;
    private readonly IAresCommandRegistry _commandRegistry;
    public override string ScriptName { get; } = "STEP_PLL";
    public override string CloserCmd { get; } = "STEP_END";
    public override int ArgCount { get; } = 1;
    public override string HelpString { get; } = "Denotes the start of a step.\n" +
                                                 "Usage: A step name may be supplied after the command.\n" +
                                                 "Ex. STEP_PLL This is the step name\n" +
                                                 "**All commands within a step are completed in parallel before the next step proceeds**";

    private bool _terminated;
    public StepStartParallelCommand(ICampaignExecutionSummary campaignExecutionSummary, ICurrentConfig currentConfig, IAresCommandRegistry commandRegistry)

    {
      _campaignExecutionSummary = campaignExecutionSummary;
      _currentConfig = currentConfig;
      _commandRegistry = commandRegistry;
    }

    public override bool Validate(string[] args)
    {
      return args.Length > 0;
    }

    public override async Task Execute(string[] lines)
    {
      _terminated = false;
      var currExp = _campaignExecutionSummary.ExperimentExecutionSummaries.Last();
      var currStep = currExp.StepExecutionSummaries.Last();
      currStep.StepName = lines[0].Substring(ScriptName.Length);
      var innerCommands = new List<string>(lines);
      innerCommands.RemoveAt(0);
      innerCommands.Remove(innerCommands.Last());
      currStep.ExecutionDuration = TimeSpan.Zero;
      // start timer
      var taskList = new List<Task>();
      var timerObservable = Observable.Interval(TimeSpan.FromSeconds(_currentConfig.TimerPrecision))
        .Subscribe(_ => currStep.ExecutionDuration = currStep.ExecutionDuration.Add(TimeSpan.FromSeconds(_currentConfig.TimerPrecision)));
      for (var index = 0; index < innerCommands.Count; index++)
      {
        if (_terminated)
        {
          return;
        }
        var commandStr = innerCommands[index];
        var lineTokens = commandStr.Split(' ');
        var cmd = lineTokens[0];
        var command = _commandRegistry.FirstOrDefault(c => c.ScriptName != null && c.ScriptName.Equals(cmd));
        var revisedTokens = new List<string>();
        for (int k = 1; k < lineTokens.Length; k++)
        {
          revisedTokens.Add(lineTokens[k]);
        }
        var cmdResult = AresKernel._kernel.Get<ICommandExecutionSummary>();
        cmdResult.Command = cmd;
        cmdResult.Status = ExecutionStatus.EXECUTING;
        cmdResult.Value = string.Join(" ", revisedTokens);
        currStep.CommandExecutionSummaries.Add(cmdResult);
        var executionTask = CreateTask(command, revisedTokens, cmdResult);

        taskList.Add(executionTask);
      }
      var taskArray = taskList.ToArray();

      try
      {
        await Task.WhenAll(taskArray);

      }
      catch (Exception)
      {

      }
      finally
      {
        timerObservable.Dispose();
        if (_retryInfos.Count == 0)
        {
          IsComplete = true;
        }
      }
    }

    private async Task CreateTask(IAresCommand command, List<string> revisedTokens, ICommandExecutionSummary cmdExecutionSummary)
    {
      var cmdErrorSub = command.WhenPropertyChanged(cmd => cmd.Error, false).Take(1).Subscribe(error => OnFail(command, revisedTokens, cmdExecutionSummary));
      var prec = TimeSpan.FromSeconds(_currentConfig.TimerPrecision);
      cmdExecutionSummary.ExecutionDuration = TimeSpan.Zero; // In case a retry is calling this
      var taskTimer = Observable.Interval(prec).Subscribe(r => cmdExecutionSummary.ExecutionDuration = cmdExecutionSummary.ExecutionDuration.Add(prec));
      await Task.Delay(TimeSpan.FromSeconds(new Random().Next() % 3));

      try
      {
        await command.Execute(revisedTokens.ToArray());
        cmdExecutionSummary.Status = ExecutionStatus.DONE;
      }
      catch (Exception)
      {
        cmdExecutionSummary.Status = ExecutionStatus.ERROR;
        // TODO: Assign an error to the command here? Maybe the command should assign an error to itself when it throws an exception.
      }
      finally
      {
        taskTimer.Dispose();
        cmdErrorSub.Dispose();
      }
    }

    protected override async Task HandleRetry()
    {
      var retryInfo = _retryInfos.FirstOrDefault();
      try
      {
        await CreateTask(retryInfo.Command, retryInfo.RevisedTokens, retryInfo.CommandExecutionSummary);
      }
      catch (Exception)
      {
      }
      _retryInfos.RemoveAt(0);
    }

    protected override Task HandleIgnoreAndContinue()
    {
      _retryInfos.RemoveAt(0);
      return Task.CompletedTask;
    }

    protected override Task HandleStop()
    {
      _retryInfos.Clear();
      _terminated = true;
      Error = new AresError { Text = "Script Handling Parallel Step" };
      return Task.CompletedTask;
    }

    public override async Task Handle(ErrorResponse response)
    {
      await base.Handle(response);
      if (_retryInfos.Count == 0 /* && response != ErrorResponse.Stop */ ) // This check is required because we notify IsComplete even if the value doesnt change, and we only take 1 notification
      {
        IsComplete = _retryInfos.Count == 0;
      }
    }

    private void OnFail(IAresCommand command, List<string> revisedTokens, ICommandExecutionSummary cmdExecutionSummary)
    {
      var retryInfo = new CommandRetryInfo { Command = command, RevisedTokens = revisedTokens, CommandExecutionSummary = cmdExecutionSummary };
      _retryInfos.Add(retryInfo);


      CreateAndAddErrorBundle(this, command);
    }

  }
}
