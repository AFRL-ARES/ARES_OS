using ARESCore.Commands;
using ARESCore.DisposePatternHelpers;
using ARESCore.ErrorSupport.Impl;
using ARESCore.ErrorSupport.Impl.RetryInfos;
using ARESCore.Experiment.Results;
using ARESCore.Extensions;
using ARESCore.Registries;
using ARESCore.UI;
using Castle.Core.Internal;
using DynamicData.Binding;
using Ninject;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ARESCore.Experiment.Scripting.Impl
{
  public class ScriptExecutor : BasicReactiveObjectDisposable, IScriptExecutor
  {
    private readonly IAresCommandRegistry _commandRegistry;
    private readonly ICampaignExecutionSummary _campaignExecutionSummary;
    private readonly List<StepRetryInfo> _retryInfos = new List<StepRetryInfo>();
    private bool _isComplete;
    private bool _terminated = false;

    public ScriptExecutor(IAresCommandRegistry commandRegistry, ICampaignExecutionSummary campaignExecutionSummary)
    {
      _commandRegistry = commandRegistry;
      _campaignExecutionSummary = campaignExecutionSummary;
    }

    public async Task Run(string script, IPlannedExperimentInputs inputs)
    {
      _terminated = false;
      var templines = CmdLines(script);
      var lines = templines;
      var currExperimentResult = _campaignExecutionSummary.ExperimentExecutionSummaries.Last();

      // get all planner values
      for (int i = 0; i < templines.Count; i++)
      {
        if (templines[i].Contains("VAL_"))
        {
          var startidx = templines[i].IndexOf("VAL_");
          var substr = templines[i].Substring(startidx).Split(' ')[0];
          double targval = inputs.Inputs[substr];
          lines[i] = templines[i].Replace(substr, targval.ToString());
        }
      }

      int j = 0;
      while (j < lines.Count && !_terminated)
      {
        var lineTokens = lines[j].Split(' ');
        var cmd = lineTokens[0];
        var command = _commandRegistry.FirstOrDefault(c => c.ScriptName != null && c.ScriptName.Equals(cmd));

        if (command.CloserCmd.IsNullOrEmpty())
        {
          List<string> revisedTokens = new List<string>();
          for (int k = 1; k < lineTokens.Length; k++)
          {
            revisedTokens.Add(lineTokens[k]);
          }
          var strayCommandSub = command.Subscribe(c => c.Error, c => OnFail(command, revisedTokens, null));
          try
          {
            await command.Execute(revisedTokens.ToArray());
          }
          catch (Exception)
          {
            // Already handled?
          }
          finally
          {
            strayCommandSub.Dispose();
          }
        }
        else // Command executes several lines of script
        {

          var subList = new List<string>();
          while (!lines[j].StartsWith(command.CloserCmd) && j < lines.Count)
          {
            subList.Add(lines[j]);
            j++;
          }
          subList.Add(lines[j]);
          var stepResult = AresKernel._kernel.Get<IStepExecutionSummary>();
          currExperimentResult.StepExecutionSummaries.Add(stepResult);
          try
          {
            if (_terminated)
            {
              IsComplete = true;
              return;
            }
            await ExecuteStep((IAresScriptCommand)command, subList, stepResult);
            do
              await Task.Delay(500);
            while (_retryInfos.Any() && !_terminated);
          }
          catch (Exception)
          {
          }
        }
        j++;
      }
      if (!_retryInfos.Any())
      {
        IsComplete = true;
      }
    }

    private async Task ExecuteStep(IAresScriptCommand command, List<string> subList, IStepExecutionSummary stepExecutionSummary)
    {
      var shouldContinue = false;
      var stepErrorSub = command.Subscribe(c => c.Error, c => OnFail(command, subList, stepExecutionSummary));
      // The CompletionUpdated function is actually required for getting the reference to the boolean. Booo
      command.WhenPropertyChanged(c => c.IsComplete, false).Take(1).Subscribe(completion => CompletionUpdated(ref shouldContinue, completion.Value));
      try
      {
        await command.Execute(subList.ToArray());
        while (!shouldContinue && !_terminated)
        {
          await Task.Delay(500);
        }
      }
      catch (Exception)
      {
        // Already handled?
      }
      finally
      {
        stepErrorSub.Dispose();
      }
    }

    private void CompletionUpdated(ref bool shouldContinue, bool completionValue)
    {
      shouldContinue = completionValue;
    }

    private void OnFail(IAresCommand command, List<string> subList, IStepExecutionSummary stepExecutionSummary)
    {
      var retryInfo = new StepRetryInfo { Step = command, SubList = subList, StepResult = stepExecutionSummary };
      _retryInfos.Add(retryInfo);

      CreateAndAddErrorBundle(this, command);
    }

    public bool Validate(string script, IPlannedExperimentInputs inputs)
    {
      var lines = CmdLines(script);

      // get all planner values
      for (int i = 0; i < lines.Count; i++)
      {
        if (lines[i].Contains("VAL_"))
        {
          var startidx = lines[i].IndexOf("VAL_");
          var substr = lines[i].Substring(startidx).Split(' ')[0];
          bool found = inputs != null && inputs.Inputs.TryGetValue(substr, out _);
          if (!found)
          {
            AresKernel._kernel.Get<IAresConsole>().WriteLine("Script Validation failed. " + substr + " is not provided in the plan.");
            return false;
          }
        }
      }
      for (int i = 0; i < lines.Count; i++)
      {
        var lineTokens = lines[i].Split(' ');
        var cmd = lineTokens[0];
        var command = _commandRegistry.FirstOrDefault(c => c.ScriptName != null && c.ScriptName.Equals(cmd));
        if (command == null)
        {
          return false;
        }

        if (!command.CloserCmd.IsNullOrEmpty())
        {
          var j = i + 1;
          while (!lines[j].StartsWith(command.CloserCmd) && j < lines.Count)
          {
            j++;
          }
          if (j == lines.Count)
          {
            AresKernel._kernel.Get<IAresConsole>().WriteLine("Script Validation failed. The command \"" + lines[i] + "\" (line " + (i + 1) + " ) is never closed.");
            return false;
          }
        }
      }
      return true;
    }

    private List<string> CmdLines(string scriptText)
    {
      scriptText = scriptText.Replace("\r", "");
      var lines = scriptText.Split('\n').ToList();

      lines.RemoveAll(s => s.Trim() == string.Empty);
      lines.RemoveAll(s => s.Trim().StartsWith("//"));

      for (int i = 0; i < lines.Count; i++)
      {
        lines[i] = lines[i].Trim();
        var index = lines[i].IndexOf("//");
        if (index > 0)
        {
          lines[i] = lines[i].Substring(0, index);
          lines[i] = lines[i].Trim();
        }
      }

      return lines;
    }

    public bool IsComplete
    {
      get => _isComplete;
      set
      {
        _isComplete = value;
        this.RaisePropertyChanged();
      }
    }

    public override async Task Handle(ErrorResponse response)
    {
      await base.Handle(response);
      if (!_retryInfos.Any())
      {
        IsComplete = true;
      }
    }

    protected override Task HandleStop()
    {
      _terminated = true;
      _retryInfos.Clear();
      Error = new AresError { Severity = ErrorSeverity.Error, Text = "Campaign Handling Script" };
      return Task.CompletedTask;
    }

    protected override async Task HandleRetry()
    {
      var retryInfo = _retryInfos.FirstOrDefault();
      try
      {
        retryInfo.StepResult.CommandExecutionSummaries.Clear();
        await ExecuteStep((IAresScriptCommand)retryInfo.Step, retryInfo.SubList, retryInfo.StepResult);
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
  }
}