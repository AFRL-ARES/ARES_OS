using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.PythonStageController.Commands
{
  public class StageControllerGotoAnalysisPositionCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "GOTO_ANALYSIS";
    public override int ArgCount { get; }

    public override string HelpString { get; } =
      "Moves the selected tool to the analysis position of the experiment cell the tool is currently at";
    public override bool Validate(string[] args)
    {
      return args.Length == ArgCount;
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
      // TODO: Add experiment tracking in the CORE campaign to allow the devices to know which experiment is current
      var currentPos = new Point(stageController.XPosition, stageController.YPosition);
      // Note: This is a workaround the "TODO" above
      var currentExperimentCellIndex = stageController.ExperimentGrid.GetIndex(currentPos);
      var experimentStartPos2D = stageController.ExperimentGrid.GetStartingPointAbsolute(currentExperimentCellIndex);
      var experimentStartPos = new Point3D(experimentStartPos2D.X, experimentStartPos2D.Y, stageController.ExperimentGrid.InitZPosition);
      var analysisPosition = experimentStartPos + new Vector3D(
                               stageController.ExperimentGrid.AnalysisStepX,
                               stageController.ExperimentGrid.AnalysisStepY,
                               stageController.ExperimentGrid.AnalysisStepZ);
      await stageController.GetPositions();
      await stageController.MoveTo(analysisPosition.X, analysisPosition.Y, analysisPosition.Z);
    }

    public override Type AssociatedDeviceType { get; set; }
  }
}
