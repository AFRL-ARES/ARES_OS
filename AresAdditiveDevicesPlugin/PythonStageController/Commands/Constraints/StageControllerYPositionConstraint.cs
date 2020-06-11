using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerYPositionConstraint : ConstrainedValue<double>
  {
    public StageControllerYPositionConstraint()
    {
      MinValue = -1000;
      MaxValue = 1000;
    }
  }
}
