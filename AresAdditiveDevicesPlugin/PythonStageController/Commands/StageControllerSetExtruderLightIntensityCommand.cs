﻿using AresAdditiveDevicesPlugin.PythonStageController.Commands.Constraints;
using ARESCore.DeviceSupport;
using System;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerSetExtruderLightIntensityCommand : AresDeviceCommand<int>
  {
    public override ConstrainedValue<int> Constraints { get; set; } = new StageControllerExtruderLightIntensityConstraint();
    public override int Value { get; set; }
    public override string ScriptName { get; }
    public override int ArgCount { get; } = 1;
    public override string HelpString { get; }
    public override bool Validate(string[] args)
    {
      if (args.Length != ArgCount)
      {
        return false;
      }
      if (double.TryParse(args[0], out var intensity))
      {
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
      var intensity = int.Parse(lines[0]);
      Value = Constraints.Constrain(intensity);
      return SubmitCommand();
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IStageController);
  }
}