using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerStepSouthWestConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerStepSouthWestConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
