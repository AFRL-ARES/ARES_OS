using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerEPositionConstraint : ConstrainedValue<double>
  {
    public StageControllerEPositionConstraint()
    {
      MinValue = -1000;
      MaxValue = 1000;
    }
  }
}
