using ARESCore.DeviceSupport;
using ARESCore.Util;
using CommonServiceLocator;
using System;
using System.Threading.Tasks;

namespace AresSampleDevicesPlugin.SampleDevice.Commands
{
  public class SampleDeviceSetDoubleCommand : AresDeviceCommand<double>
  {
    public override string ScriptName { get; } = "SAMPLE_SETDOUBLE";
    public override int ArgCount { get; } = 1;
    public override ConstrainedValue<double> Constraints { get; set; } = new SampleDeviceDoubleConstrainedValue();
    public override Enum UnitType { get; set; } = Power.Watt;
    public override double Value { get; set; }
    public override bool IsWriteOnly { get; } = true;
    public override string Serialize()
    {
      return "P=" + Value + "\r\n";
    }

    public override void Deserialize(string val)
    {
    }

    public override Task Execute(string[] args)
    {
      //Error = new AresError { Severity = ErrorSeverity.Error, Text = "Step Handling Command" };
      var x = double.Parse(args[0]);

      Value = Constraints.Constrain(x);

      var sampleDevice = ServiceLocator.Current.GetInstance<ISampleDevice>();
      sampleDevice.SetDouble(Value);

      return Task.CompletedTask;

    }

    public override string HelpString { get; } = "Usage: Provide a single double representing the desired value.\n" +
                                               "Ex: SAMPLE_SETDOUBLE 1.2 (Set the value to 1.2 Watts)." +
                                               "Ex. SAMPLE_SETDOUBLE VAL_SAMPLE (Set the value to the experiment's desired value).";

    public override bool Validate(string[] args)
    {
      if (args.Length != ArgCount)
        return false;
      if (double.TryParse(args[0], out _))
        return true;
      if (args[0].Equals(PlanValueString))
        return true;
      return false;
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(ISampleDevice);

    public override bool IsPlannable { get; set; } = true;

    public override string PlanValueString { get; set; } = "VAL_SAMPLE";
  }
}
