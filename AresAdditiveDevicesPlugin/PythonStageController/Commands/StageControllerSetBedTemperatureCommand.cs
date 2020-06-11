using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints;
using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerSetBedTemperatureCommand : AresDeviceCommand<double>
  {
    public override ConstrainedValue<double> Constraints { get; set; } = new StageControllerSetBedTemperatureConstrainedValue();
    public override double Value { get; set; }
    public override string ScriptName { get; } = "SET_BED_TEMPERATURE";
    public override string PlanValueString { get; set; } = "VAL_BED_TEMPERATURE";
    public override bool IsPlannable { get; set; } = true;
    public override int ArgCount { get; } = 1;
    public override string HelpString { get; } = "Sets the python configuration value for the bed temperature";
    public override bool Validate(string[] args)
    {
      if (args.Length != ArgCount)
      {
        return false;
      }
      if (double.TryParse(args[0], out var value))
      {
        return true;
      }
      return false;
    }

    public override string Serialize()
    {
      throw new NotImplementedException();
    }

    public override void Deserialize(string val)
    {
      throw new NotImplementedException();
    }

    public override Task Execute(string[] lines)
    {
      // This is actually true
      throw new NotImplementedException();
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
