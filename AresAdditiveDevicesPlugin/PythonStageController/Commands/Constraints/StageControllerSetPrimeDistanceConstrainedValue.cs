using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerSetPrimeDistanceConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerSetPrimeDistanceConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
