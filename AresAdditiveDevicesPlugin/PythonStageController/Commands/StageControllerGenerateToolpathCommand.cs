using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerGenerateToolpathCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "GENERATE_TOOLPATH";
    public override int ArgCount { get; }
    public override string HelpString { get; } = "Generates the toolpath with the selected filename (TODO: allow a filepath argument in this command rather than depend on the main window?)";
    public override bool Validate(string[] args)
    {
      return args.Length == ArgCount;
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
      return stageController.GenerateToolpath();
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
