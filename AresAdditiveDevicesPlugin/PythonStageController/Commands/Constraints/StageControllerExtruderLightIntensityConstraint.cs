using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerExtruderLightIntensityConstraint : ConstrainedValue<int>
  {
    public StageControllerExtruderLightIntensityConstraint()
    {
      MinValue = 0;
      MaxValue = 255;
    }
  }
}
