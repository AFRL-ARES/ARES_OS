using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class ExperimentStatus : BasicReactiveObjectDisposable
  {
    private int _expNum;
    private string _status;

    public int ExpNum
    {
      get => _expNum;
      set => this.RaiseAndSetIfChanged(ref _expNum, value);
    }

    public string Status
    {
      get => _status;
      set => this.RaiseAndSetIfChanged(ref _status, value);
    }
  }
}
