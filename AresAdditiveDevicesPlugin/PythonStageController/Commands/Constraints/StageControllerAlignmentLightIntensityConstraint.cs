using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerAlignmentLightIntensityConstraint : ConstrainedValue<int>
  {
    public StageControllerAlignmentLightIntensityConstraint()
    {
      MinValue = 0;
      MaxValue = 255;
    }
  }
}
