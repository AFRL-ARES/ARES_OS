using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerSetNozzleDiameterConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerSetNozzleDiameterConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
