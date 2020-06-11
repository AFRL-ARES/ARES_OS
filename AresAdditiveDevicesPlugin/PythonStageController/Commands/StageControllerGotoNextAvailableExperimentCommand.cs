using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ARESCore.DeviceSupport;
using ARESCore.ErrorSupport.Impl;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerGotoNextAvailableExperimentCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "GOTO_AVAILABLE";
    public override int ArgCount { get; }

    public override string HelpString { get; } =
      "Moves the selected tool to the nearest progressive experiment cell.\n" +
      "If the tool is over a valid cell already, the current cell will be used.";
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
      if (stageController.ExperimentGrid.All(experimentCellValid => !experimentCellValid))
      {
        Error = new AresError { Severity = ErrorSeverity.Error, Text = "Could not locate an available experiment cell position" };
      }
      await stageController.GetPositions();
      await stageController.StepZ(2);
      //      var currentPosition = new Point(stageController.XPosition, stageController.YPosition);
      //      var availableIndex = stageController.ExperimentGrid.NextAvailableIndex(currentPosition);
      var availableIndex = stageController.ExperimentGrid.NextAvailableIndex(new Point(stageController.ExperimentGrid.InitXPosition, stageController.ExperimentGrid.InitYPosition));
      var experimentStart = stageController.ExperimentGrid.GetStartingPointAbsolute(availableIndex);
      await stageController.MoveTo(experimentStart.X, experimentStart.Y, stageController.ExperimentGrid.InitZPosition);
    }

    public override Type AssociatedDeviceType { get; set; }
  }
}
