using System.Linq;
using System.Reactive;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.PythonStageController.UI.Vms
{
  public class ExperimentGridViewModel : BasicReactiveObjectDisposable
  {
    private IStageController _stageController;

    public ExperimentGridViewModel(IStageController stageController)
    {
      StageController = stageController;
      UseTopLeftCommand = ReactiveCommand.Create(StageController.SetTopLeft);
      UseTopRightCommand = ReactiveCommand.Create(StageController.SetTopRight);
      UseBottomLeftCommand = ReactiveCommand.Create(StageController.SetBottomLeft);
      UseBottomRightCommand = ReactiveCommand.Create(StageController.SetBottomRight);
      UseCurrentXCommand = ReactiveCommand.Create(SetCurrentX);
      UseCurrentYCommand = ReactiveCommand.Create(SetCurrentY);
      UseCurrentZCommand = ReactiveCommand.Create(SetCurrentZ);
      UseCurrentECommand = ReactiveCommand.Create(SetCurrentE);
      SetInitialPositionCommand = ReactiveCommand.Create<int>(StageController.SetInitialPositionAt);
      ConfirmGenerateExtentsCommand = ReactiveCommand.Create(ConfirmGenerateExtents);
    }

    private void SetCurrentX()
    {
      StageController.ExperimentGrid.InitXPosition = StageController.XPosition;
    }

    private void SetCurrentY()
    {
      StageController.ExperimentGrid.InitYPosition = StageController.YPosition;
    }

    private void SetCurrentZ()
    {
      StageController.ExperimentGrid.InitZPosition = StageController.ZPosition;
    }

    private void SetCurrentE()
    {
      StageController.ExperimentGrid.InitExtruderPosition = StageController.EPosition;
    }

    private async void ConfirmGenerateExtents()
    {
      if (!StageController.ExperimentGrid.Any())
      {
        await StageController.GenerateGridExtents();
        return;
      }
      //      var result = await Coordinator.ShowMessageAsync(this, "Warning", "Generating new extents will reset the grid values, allowing each index to have an experiment run", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AnimateShow = false });
      //      if (result == MessageDialogResult.Affirmative)
      //      {
      //        await StageController.ExperimentGrid.GenerateExtents();
      //      }
    }

    public IStageController StageController
    {
      get => _stageController;
      set => this.RaiseAndSetIfChanged(ref _stageController, value);
    }

    public ReactiveCommand<Unit, Unit> UseTopLeftCommand { get; }
    public ReactiveCommand<Unit, Unit> UseTopRightCommand { get; }
    public ReactiveCommand<Unit, Unit> UseBottomLeftCommand { get; }
    public ReactiveCommand<Unit, Unit> UseBottomRightCommand { get; }
    public ReactiveCommand<Unit, Unit> UseCurrentXCommand { get; }
    public ReactiveCommand<Unit, Unit> UseCurrentYCommand { get; }
    public ReactiveCommand<Unit, Unit> UseCurrentZCommand { get; }
    public ReactiveCommand<Unit, Unit> UseCurrentECommand { get; }
    public ReactiveCommand<int, Unit> SetInitialPositionCommand { get; }
    public ReactiveCommand<Unit, Unit> ConfirmGenerateExtentsCommand { get; }
  }
}
