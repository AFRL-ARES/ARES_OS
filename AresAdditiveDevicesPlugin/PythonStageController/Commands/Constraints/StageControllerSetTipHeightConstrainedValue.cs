using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerSetTipHeightConstrainedValue : ConstrainedValue<double>
  {
    public StageControllerSetTipHeightConstrainedValue()
    {
      MinValue = 0;
      MaxValue = 1000;
    }
  }
}
