using System;
using System.Threading.Tasks;
using System.Windows;
using ARESCore.DeviceSupport;
using ARESCore.ErrorSupport.Impl;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerValidateExperimentCellAvailability : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "VALIDATE_AVAILABILITY";
    public override int ArgCount { get; }
    public override string HelpString { get; } = "Errors out if the selected tool is not at location available for experimentation";
    public override bool Validate(string[] args)
    {
      return args.Length == 0;
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
      await stageController.GetPositions();
      var currentPosition = new Point(stageController.XPosition, stageController.YPosition);
      if (!stageController.ExperimentGrid[currentPosition])
      {
        Error = new AresError() { Severity = ErrorSeverity.Error, Text = "Failed experiment cell availability check. Cannot perform experiment at the current location." };
      }
    }

    public override Type AssociatedDeviceType { get; set; }
  }
}
