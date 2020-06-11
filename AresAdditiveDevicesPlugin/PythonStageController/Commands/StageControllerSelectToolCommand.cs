using System;
using System.Linq;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.ErrorSupport.Impl;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerSelectToolCommand : AresDeviceCommand<string>
  {
    private readonly string[] _availableToolNames = new[] { "EXTRUDER", "ALIGNMENT" };
    public override ConstrainedValue<string> Constraints { get; set; }
    public override string Value { get; set; }
    public override string ScriptName { get; } = "SELECT_TOOL";
    public override int ArgCount { get; } = 1;
    public override string HelpString { get; } = "Select the extruder tool or alignment tool";
    public override bool Validate(string[] args)
    {
      return args.Length == 1 &&
             _availableToolNames.Any(toolName => toolName.Equals(args[0], StringComparison.CurrentCultureIgnoreCase));
    }

    public override string Serialize()
    {
      throw new NotImplementedException();
    }

    public override void Deserialize(string val)
    {
      throw new NotImplementedException();
    }

    public override Task Execute(string[] lines)
    {
      var stageController = ServiceLocator.Current.GetInstance<IStageController>();
      Value = lines[0];
      if (Value.Equals(_availableToolNames[0], StringComparison.CurrentCultureIgnoreCase))
      {
        return stageController.SelectExtruderTool();
      }
      else if (Value.Equals(_availableToolNames[1], StringComparison.CurrentCultureIgnoreCase))
      {
        return stageController.SelectAlignmentTool();
      }
      else
      {
        Error = new AresError() { Severity = ErrorSeverity.Error, Text = $"Could not select tool: {Value}" };
        return SubmitCommand();
      }
    }

    public override Type AssociatedDeviceType { get; set; }
  }
}
