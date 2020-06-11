using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerSetHomeCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "SET_HOME";
    public override int ArgCount { get; }

    public override string HelpString { get; } =
      "Sets the stage controller's home position to the selected tool's current position";
    public override bool Validate(string[] args)
    {
      return args.Length == 0;
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
      var stageController = ServiceLocator.Current.GetInstance<IStageController>();
      return stageController.SetHome();
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
