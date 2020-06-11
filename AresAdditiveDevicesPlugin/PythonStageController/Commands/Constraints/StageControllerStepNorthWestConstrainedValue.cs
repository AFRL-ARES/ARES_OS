using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerStepNorthWestConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerStepNorthWestConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
