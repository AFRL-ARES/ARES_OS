using AresAdditiveDevicesPlugin.PythonStageController.Impl;
using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints
{
  public class StageControllerMoveToConstraint : ConstrainedValue<Location>
  {
    public override Location Constrain(Location location)
    {
      var xConstraint = new StageControllerXPositionConstraint();
      var yConstraint = new StageControllerYPositionConstraint();
      var zConstraint = new StageControllerZPositionConstraint();
      var eConstraint = new StageControllerEPositionConstraint();
      return new Location
      {
        X = xConstraint.Constrain(location.X),
        Y = yConstraint.Constrain(location.Y),
        Z = zConstraint.Constrain(location.Z),
        E = eConstraint.Constrain(location.E)
      };
    }
  }
}
