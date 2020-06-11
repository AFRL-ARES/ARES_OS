using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerSetExtrusionMultiplierConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerSetExtrusionMultiplierConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
