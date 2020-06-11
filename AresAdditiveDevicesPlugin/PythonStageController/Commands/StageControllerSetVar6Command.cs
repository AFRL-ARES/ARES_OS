using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerSetVar6Command : AresDeviceCommand<double>
  {
    public override ConstrainedValue<double> Constraints { get; set; } = new StageControllerSetVar1ConstrainedValue();
    public override double Value { get; set; }
    public override string ScriptName { get; } = "SET_VAR6";
    public override string PlanValueString { get; set; } = "VAL_VAR6";
    public override bool IsPlannable { get; set; } = true;
    public override int ArgCount { get; } = 1;
    public override string HelpString { get; } = "Sets the python configuration value for custom variable 6";
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
      await stageController.WritePyDict("uservars.var6", value);
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}
