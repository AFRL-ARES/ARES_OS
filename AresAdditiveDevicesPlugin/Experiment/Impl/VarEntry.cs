using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Experiment.Impl
{
  public class VarEntry : BasicReactiveObjectDisposable
  {
    private string _name;
    private double _value;
    private double _min;
    private double _max;

    public string Name
    {
      get => _name;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public double Value
    {
      get => _value;
      set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public double Min
    {
      get => _min;
      set => this.RaiseAndSetIfChanged(ref _min, value);
    }

    public double Max
    {
      get => _max;
      set => this.RaiseAndSetIfChanged(ref _max, value);
    }
  }
}
