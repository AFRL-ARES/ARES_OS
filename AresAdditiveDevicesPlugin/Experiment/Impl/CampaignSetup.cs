using System.Reactive;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonStageController;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class CampaignSetup : BasicReactiveObjectDisposable, ICampaignSetup
  {
    private IExperimentGrid _grid;

    public CampaignSetup(IStageController stageController, IExperimentGrid grid, IToolpathParameters toolpathParameters)
    {
      VarEntries = toolpathParameters;
      Grid = grid;
      UseCurrentXCommand = ReactiveCommand.Create(() => Grid.InitXPosition = stageController.XPosition);
      UseCurrentYCommand = ReactiveCommand.Create(() => Grid.InitYPosition = stageController.YPosition);
      UseCurrentZCommand = ReactiveCommand.Create(() => Grid.InitZPosition = stageController.ZPosition);
      UseCurrentECommand = ReactiveCommand.Create(() => Grid.InitExtruderPosition = stageController.EPosition);
      //      GenerateExtents = ReactiveCommand.Create(Grid.GenerateExtents);
      UseTopLeftCommand = ReactiveCommand.Create(SetTopLeft);
      UseTopRightCommand = ReactiveCommand.Create(SetTopRight);
      UseBottomLeftCommand = ReactiveCommand.Create(SetBottomLeft);
      UseBottomRightCommand = ReactiveCommand.Create(SetBottomRight);
    }

    public void SetInitialPositionAt(int index)
    {
      var xyPos = Grid.GetStartingPointIdle(index);
      Grid.InitXPosition = xyPos.X;
      Grid.InitYPosition = xyPos.Y;
    }

    private void SetTopLeft()
    {
      Grid.InitXPosition = Grid.LimitWest;
      Grid.InitYPosition = Grid.LimitNorth;
    }

    private void SetTopRight()
    {
      Grid.InitXPosition = Grid.LimitEast;
      Grid.InitYPosition = Grid.LimitNorth;
    }

    private void SetBottomLeft()
    {
      Grid.InitXPosition = Grid.LimitWest;
      Grid.InitYPosition = Grid.LimitSouth;
    }

    private void SetBottomRight()
    {
      Grid.InitXPosition = Grid.LimitEast;
      Grid.InitYPosition = Grid.LimitSouth;
    }

    public IExperimentGrid Grid
    {
      get => _grid;
      set => this.RaiseAndSetIfChanged(ref _grid, value);
    }

    public ReactiveCommand<Unit, Unit> GenerateGridCommand { get; set; }
    public ReactiveCommand<Unit, double> UseCurrentXCommand { get; set; }
    public ReactiveCommand<Unit, double> UseCurrentYCommand { get; set; }
    public ReactiveCommand<Unit, double> UseCurrentZCommand { get; set; }
    public ReactiveCommand<Unit, double> UseCurrentECommand { get; set; }
    public ReactiveCommand<Unit, Task> GenerateExtents { get; set; }


    public ReactiveCommand<Unit, Unit> UseTopLeftCommand { get; }
    public ReactiveCommand<Unit, Unit> UseTopRightCommand { get; }
    public ReactiveCommand<Unit, Unit> UseBottomLeftCommand { get; }
    public ReactiveCommand<Unit, Unit> UseBottomRightCommand { get; }

    public IToolpathParameters VarEntries { get; set; }
  }
}
