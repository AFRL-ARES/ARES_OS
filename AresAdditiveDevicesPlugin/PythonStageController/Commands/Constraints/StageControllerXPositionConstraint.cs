using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerXPositionConstraint : ConstrainedValue<double>
  {
    public StageControllerXPositionConstraint()
    {
      MinValue = -1000;
      MaxValue = 1000;
    }
  }
}
