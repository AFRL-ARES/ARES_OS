using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerRunToolpathCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "RUN_TOOLPATH";
    public override int ArgCount { get; }
    public override string HelpString { get; } = "Executes the generated toolpath";
    public override bool Validate(string[] args)
    {
      return args.Length == ArgCount;
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
      return stageController.RunToolpath();
    }

    public override Type AssociatedDeviceType { get; set; }
  }
}
