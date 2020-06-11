using System.Windows;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.PythonStageController
{
  public interface IExperimentGrid : IObservableCollection<bool>
  {
    int Rows { get; set; }
    int Columns { get; set; }
    double ExtentX { get; set; }
    double ExtentY { get; set; }
    double SeparationX { get; set; }
    double SeparationY { get; set; }
    double AnalysisStepX { get; set; }
    double AnalysisStepY { get; set; }
    double AnalysisStepZ { get; set; }
    double ExtentZ { get; set; }
    double LimitNorth { get; set; }
    double LimitWest { get; set; }
    double LimitSouth { get; set; }
    double LimitEast { get; set; }
    double InitXPosition { get; set; }
    double InitYPosition { get; set; }
    double InitZPosition { get; set; }
    double InitExtruderPosition { get; set; }
    int ObservableCount { get; set; }

    bool this[int x, int y] { get; set; }
    bool this[Point location] { get; set; }

    Point GetStartingPointAbsolute(int x, int y);
    Point GetStartingPointIdle(int x, int y);
    Point GetStartingPointAbsolute(int index);
    Point GetStartingPointIdle(int index);
    Point GetStartingPointRelative(int x, int y);
    Point GetStartingPointRelative(int index);
    Point GetStartingPointRelative(Point currentPoint);
    int GetIndex(Point currentPoint);
    int NextAvailableIndex(Point currentPoint);
    void GenerateGrid();
  }
}
