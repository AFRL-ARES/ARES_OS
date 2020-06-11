using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerSetNozzleDiameterCommand : AresDeviceCommand<double>
  {
    public override ConstrainedValue<double> Constraints { get; set; } = new StageControllerSetNozzleDiameterConstrainedValue();
    public override double Value { get; set; }
    public override string ScriptName { get; } = "SET_NOZZLE_DIAMETER";
    public override string PlanValueString { get; set; } = "VAL_NOZZLE_DIAMETER";
    public override bool IsPlannable { get; set; } = true;
    public override int ArgCount { get; } = 1;
    public override string HelpString { get; } = "Sets the python configuration value for nozzle diameter";
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

    public override async Task Execute(string[] lines)
    {
      var stageController = ServiceLocator.Current.GetInstance<IStageController>();
      var value = double.Parse(lines[0]);
      await stageController.WritePyDict("dispenser.diameter", value);
      await stageController.WritePyDict("dispenser.radius", Math.Round(value / 2, 6));
      await stageController.WritePyDict("dispenser.tip_volume", Math.PI * Math.Pow(value / 2, 2));
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
