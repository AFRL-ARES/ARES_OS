using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerTranslateYConstraint : ConstrainedValue<double>
  {
    public StageControllerTranslateYConstraint()
    {
      MinValue = -1000;
      MaxValue = 1000;
    }
  }
}
