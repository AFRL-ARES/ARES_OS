using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints;
using AresAdditiveDevicesPlugin.PythonStageController.Impl;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerGetPositionsCommand : AresDeviceCommand<Location>
  {
    public override ConstrainedValue<Location> Constraints { get; set; } = new StageControllerMoveToConstraint();
    public override Location Value { get; set; }
    public override string ScriptName { get; } = "UPDATE_POSITIONS";
    public override int ArgCount { get; }
    public override string HelpString { get; } = "Requests and updates the current X,Y,Z, and E positions of the selected tool";
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
      return stageController.GetPositions();
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
