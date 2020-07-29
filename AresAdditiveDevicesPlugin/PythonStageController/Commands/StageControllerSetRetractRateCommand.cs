﻿using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerSetRetractRateCommand : AresDeviceCommand<double>
  {
    public override ConstrainedValue<double> Constraints { get; set; } = new StageControllerSetNozzleDiameterConstrainedValue();
    public override double Value { get; set; }
    public override string ScriptName { get; } = "SET_RETRACT_RATE";
    public override string PlanValueString { get; set; } = "VAL_RETRACT_RATE";
    public override bool IsPlannable { get; set; } = true;
    public override int ArgCount { get; } = 1;
    public override string HelpString { get; } = "Sets the python configuration value for retract rate";
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
      await stageController.WritePyDict("dispenser.retract_rate", value);
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}