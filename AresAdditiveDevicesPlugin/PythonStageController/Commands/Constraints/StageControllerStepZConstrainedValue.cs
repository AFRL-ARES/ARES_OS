using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerStepZConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerStepZConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
