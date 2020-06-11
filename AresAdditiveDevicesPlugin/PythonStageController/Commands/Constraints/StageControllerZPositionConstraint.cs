using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerZPositionConstraint : ConstrainedValue<double>
  {
    public StageControllerZPositionConstraint()
    {
      MinValue = -100;
      MaxValue = 1000;
    }
  }
}
