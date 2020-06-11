using AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints;
using ARESCore.DeviceSupport;
using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController.Impl;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerMoveToCommand : AresDeviceCommand<Location>
  {
    public override ConstrainedValue<Location> Constraints { get; set; } = new StageControllerMoveToConstraint();
    public override Location Value { get; set; }
    public override string ScriptName { get; }
    public override int ArgCount { get; } = 2;
    public override string HelpString { get; }
    public override bool Validate(string[] args)
    {
      if (args.Length < ArgCount)
      {
        return false;
      }
      if (double.TryParse(args[0], out var x) &&
        double.TryParse(args[1], out var y))
      {
        if (args[2] != null)
        {
          if (!double.TryParse(args[2], out var z))
          {
            return false;
          }
          if (args[3] != null)
          {
            if (!double.TryParse(args[3], out var e))
            {
              return false;
            }
            {
              return true;
            }
          }
        }
        return true;
      }
      return args[0].Equals(PlanValueString);
    }

    public override string Serialize()
    {
      return string.Empty;
    }

    public override void Deserialize(string val)
    {
    }

    public override Task Execute(string[] lines)
    {
      var location = Location.Parse(lines);
      Value = Constraints.Constrain(location);
      return SubmitCommand();
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
