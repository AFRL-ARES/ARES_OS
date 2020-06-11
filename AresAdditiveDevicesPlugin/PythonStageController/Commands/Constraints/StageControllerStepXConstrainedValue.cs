using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerStepXConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerStepXConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
