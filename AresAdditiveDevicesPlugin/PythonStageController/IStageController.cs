using System.Threading.Tasks;
using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.PythonStageController
{
  public interface IStageController : IAresDevice
  {
    bool IsInitialized { get; set; }
    double XYStepSizeSetting { get; set; }
    int XYSpeedSetting { get; set; }
    double ZStepSizeSetting { get; set; }
    int ZSpeedSetting { get; set; }
    Task StepX(double dist);
    Task StepY(double dist);
    Task StepZ(double dist);
    string ErrorText { get; set; }
    double XPosition { get; set; }
    double YPosition { get; set; }
    double ZPosition { get; set; }
    double EPosition { get; set; }
    int ExtruderLightIntensity { get; set; }
    int AlignmentLightIntensity { get; set; }
    Task Home();
    Task Level();
    Task MaintenancePosition();
    Task EStop();
    Task SetHome();
    Task ChangeTip();
    Task CleanTip();
    Task Prime(double distance, double rate);
    Task Retract(double distance, double rate);
    Task GenerateToolpath();
    Task OffsetDefinition();
    Task RemoveSample();
    Task GoHome();
    Task StepSouthEast(double dist);
    Task StepNorthEast(double dist);
    Task StepSouthWest(double dist);
    Task StepNorthWest(double dist);
    Task TranslateX(double dist);
    Task TranslateY(double dist);
    Task TranslateZ(double dist);

    Task SelectAlignmentTool();
    Task SelectExtruderTool();
    Task GetPositions();
    Task MoveTo(float x, float y, float z);
    Task MoveTo(double x, double y, double z);
    IExperimentGrid ExperimentGrid { get; }
    Task GenerateGridExtents();
    void SetInitialPositionAt(int index);
    void SetTopRight();
    void SetTopLeft();
    void SetBottomRight();
    void SetBottomLeft();
    Task RunToolpath();
    Task WritePyDict(string varName, double value);
    Task WritePyDict(string varName, string value);
    Task Abort();
  }
}
