using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ReactiveUI;

namespace ARESCore.Experiment.impl
{
  public class ExperimentParameter : ReactiveObject
  {
    private double _value;
    private string _name;
    private ConstrainedValue<double> _valueConstraints = new ConstrainedValue<double>();
    private bool _isPlanned;
    private string _unit = "na";

    public double Value
    {
      get => _value;
      set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public ConstrainedValue<double> ValueConstraints
    {
      get => _valueConstraints;
      set => this.RaiseAndSetIfChanged(ref _valueConstraints, value);
    }

    public string Name
    {
      get => _name;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public bool IsPlanned
    {
      get => _isPlanned;
      set => this.RaiseAndSetIfChanged(ref _isPlanned, value);
    }

    // TODO: Start using Units.NET
    public string Unit
    {
      get => _unit;
      set => this.RaiseAndSetIfChanged(ref _unit, value);
    }
  }
}
