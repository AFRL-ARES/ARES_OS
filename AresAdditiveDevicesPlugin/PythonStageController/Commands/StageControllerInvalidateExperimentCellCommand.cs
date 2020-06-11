using System;
using System.Threading.Tasks;
using System.Windows;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerInvalidateExperimentCellCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "INVALIDATE_CELL";
    public override int ArgCount { get; }

    public override string HelpString { get; } =
      "Sets the cell the selected tool is currently at to unavailable for experimentation";
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

    public override async Task Execute(string[] lines)
    {
      var stageController = ServiceLocator.Current.GetInstance<IStageController>();
      await stageController.GetPositions();
      var currentPos = new Point(stageController.XPosition, stageController.YPosition);
      stageController.ExperimentGrid[currentPos] = false;
    }

    public override Type AssociatedDeviceType { get; set; }
  }
}
