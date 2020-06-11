using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerTranslateZConstraint : ConstrainedValue<double>
  {
    public StageControllerTranslateZConstraint()
    {
      MinValue = -1000;
      MinValue = 1000;
    }
  }
}
