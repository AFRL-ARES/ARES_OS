using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerStepSouthEastConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerStepSouthEastConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
