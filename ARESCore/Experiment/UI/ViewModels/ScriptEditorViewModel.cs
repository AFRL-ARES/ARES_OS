using ARESCore.Commands;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Scripting;
using ARESCore.Registries;
using Ninject;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Experiment.UI.ViewModels
{
  public class ScriptEditorViewModel : BasicReactiveObjectDisposable
  {
    private readonly IAresCommandRegistry _commandRepo;
    private string _lineNumbers;
    private bool _hasError;
    private readonly ICampaign _campaign;

    public ScriptEditorViewModel()
    {
      _commandRepo = AresKernel._kernel.Get<IAresCommandRegistry>();
      _campaign = AresKernel._kernel.Get<ICampaign>();
      GetAllCommands();
    }

    public List<IAresCommand> MainScriptCommands { get; } = new List<IAresCommand>();

    public List<IAresCommand> DeviceScriptCommands { get; } = new List<IAresCommand>();

    private void GetAllCommands()
    {
      MainScriptCommands.Clear();
      DeviceScriptCommands.Clear();
      foreach (var command in _commandRepo)
      {
        if (command is IAresScriptCommand)
          MainScriptCommands.Add(command);
        else
          DeviceScriptCommands.Add(command);
      }
    }

    public bool HasError
    {
      get => _hasError;
      set
      {
        this.RaiseAndSetIfChanged(ref _hasError, value);
        _campaign.CanRunMask = (byte)(_hasError ? _campaign.CanRunMask & (byte)CampaignCanRunMask.InvalidExperimentScript : _campaign.CanRunMask | (byte)CampaignCanRunMask.ValidExperimentScript);
      }
    }

    public void LiveParse(string textEditorText)
    {
      string text = textEditorText.Replace("\r", "");
      string[] lines = text.Split('\n');
      LineNumbers = "";

      for (int i = 0; i < lines.Length; i++)
      {
        var line = lines[i].Trim(); // remove all leading and trailing whitespace
        if (line.StartsWith("//") || line.Length == 0)
        {
          LineNumbers += (i + 1) + "\r\n";
          continue;
        }
        var lineTokens = line.Split(' ');
        var cmd = lineTokens[0];
        var command = MainScriptCommands.FirstOrDefault(c => c.ScriptName != null && c.ScriptName.Equals(cmd));
        if (command == null)
          command = DeviceScriptCommands.FirstOrDefault(c => c.ScriptName != null && c.ScriptName.Equals(cmd));
        if (command == null)
        {
          LineNumbers += "ERROR\r\n";
          continue;
        }
        if (command.ArgCountEnforced)
        {
          if (lineTokens.Length < command.ArgCount + 1)
          {
            LineNumbers += "ERROR\r\n";
            continue;
          }

          if (ValidateCommand(command, lineTokens))
          {
            LineNumbers += (i + 1) + "\r\n";
            continue;
          }
          if (command.IsPlannable && lineTokens.Length == 2 && lineTokens[1].Equals(command.PlanValueString))
          {
            LineNumbers += (i + 1) + "\r\n";
            continue;
          }
          LineNumbers += "ERROR\r\n";
          continue;
        }
        LineNumbers += (i + 1) + "\r\n";
      }
    }

    private bool ValidateCommand(IAresCommand cmd, string[] lineTokens)
    {
      List<string> revisedTokens = new List<string>();
      bool ignore = false;
      for (int i = 1; i < lineTokens.Length; i++)
      {
        if (lineTokens[i].StartsWith("//"))
        {
          ignore = true;
        }
        if (!ignore)
          revisedTokens.Add(lineTokens[i]);
      }
      if (!cmd.Validate(revisedTokens.ToArray()))
      {
        return false;
      }
      return true;
    }

    public string LineNumbers
    {
      get { return _lineNumbers; }
      set { this.RaiseAndSetIfChanged(ref _lineNumbers, value); }
    }

  }
}