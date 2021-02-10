using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.Util;
using CommonServiceLocator;

namespace AresSampleDevicesPlugin.SampleDevice.Commands
{
  public class SampleDeviceGetDoubleCommand : AresDeviceCommand<double>
  {
    private double _value;
    public override string ScriptName { get; } = "SAMPLE_GETVAL";
    public override int ArgCount { get; } = 0;
    public override ConstrainedValue<double> Constraints { get; set; } = new SampleDeviceDoubleConstrainedValue();
    public override Enum UnitType { get; set; } = Power.Watt;

    public override double Value
    {
      get => _value;
      set
      {
        if (CheckLimits(value))
          _value = value;
      }
    }

    public override bool IsWriteOnly { get; } = false;

    public override string Serialize()
    {
      return "?SP\r\n";
    }

    public override void Deserialize(string val)
    {
      if (val.Length > 4)
        Value = double.Parse(val.Substring(0, 4));
    }

    public override Task Execute(string[] args)
    {
      var sampleDevice = ServiceLocator.Current.GetInstance<ISampleDevice>();
      var val = sampleDevice.DoubleValue;
      Value = val;

      return Task.CompletedTask;
    }

    public override string HelpString { get; } = "";
    public override bool Validate(string[] args)
    {
      if (args.Length != 0)
        return false;
      return true;
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(ISampleDevice);

    public override bool CheckLimits(double value)
    {
      if (value >= Constraints.MinValue && value < Constraints.MaxValue)
      {
        return true;
      }
      return false;
    }
  }
}
