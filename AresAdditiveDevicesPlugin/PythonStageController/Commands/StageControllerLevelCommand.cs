using ARESCore.DeviceSupport;
using System;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerLevelCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; }
    public override int ArgCount { get; }
    public override string HelpString { get; }
    public override bool Validate(string[] args)
    {
      return true;
    }

    public override string Serialize()
    {
      return string.Empty;
    }

    public override void Deserialize(string val)
    {

    }

    public override Task Execute(string[] lines)
    {
      return SubmitCommand();
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
