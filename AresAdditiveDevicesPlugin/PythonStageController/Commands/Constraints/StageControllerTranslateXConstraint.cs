using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerTranslateXConstraint : ConstrainedValue<double>
  {
    public StageControllerTranslateXConstraint()
    {
      MinValue = -1000;
      MaxValue = 1000;
    }
  }
}
