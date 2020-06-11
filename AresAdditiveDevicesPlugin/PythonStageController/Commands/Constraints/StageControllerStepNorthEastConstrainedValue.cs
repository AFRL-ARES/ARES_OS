using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerStepNorthEastConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerStepNorthEastConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
