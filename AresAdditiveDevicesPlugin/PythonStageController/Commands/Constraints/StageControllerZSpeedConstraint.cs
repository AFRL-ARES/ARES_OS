using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerZSpeedConstraint : ConstrainedValue<int>
  {
    public StageControllerZSpeedConstraint()
    {
      MinValue = -1000;
      MaxValue = 1000;
    }
  }
}
