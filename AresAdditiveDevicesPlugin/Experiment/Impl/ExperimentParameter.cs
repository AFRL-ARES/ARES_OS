using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class ExperimentParameter : BasicReactiveObjectDisposable, IExperimentParameter
  {
    private bool _isPlanned;
    private VarEntry _varEntry;


    public ExperimentParameter(string name, double min, double max)
    {
      VarEntry = new VarEntry { Min = min, Max = max, Name = name };
    }

    public ExperimentParameter(VarEntry varEntry)
    {
      VarEntry = varEntry;
    }


    public bool IsPlanned
    {
      get => _isPlanned;
      set => this.RaiseAndSetIfChanged(ref _isPlanned, value);
    }

    public VarEntry VarEntry
    {
      get => _varEntry;
      set => this.RaiseAndSetIfChanged(ref _varEntry, value);
    }
  }
}
