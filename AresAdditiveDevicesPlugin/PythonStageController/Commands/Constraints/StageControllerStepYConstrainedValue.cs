using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerStepYConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerStepYConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
