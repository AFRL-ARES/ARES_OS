using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerXYSpeedConstraint : ConstrainedValue<int>
  {
    public StageControllerXYSpeedConstraint()
    {
      MinValue = -1000;
      MaxValue = 1000;
    }
  }
}
