using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresSampleDevicesPlugin.SampleDevice.Commands
{
  public class SampleDeviceSetBoolComand : AresDeviceCommand<bool>
  {
    private readonly List<string> _trueEntries = new List<string>() { "true", "on", "open", "yes", "1" };
    private readonly List<string> _validEntries = new List<string>() { "true", "on", "open", "yes", "1", "false", "off", "closed", "no", "0" };
    public override string ScriptName { get; } = "SAMPLE_SETBOOL";
    public override int ArgCount { get; } = 1;
    public override ConstrainedValue<bool> Constraints { get; set; } = null;
    public override Enum UnitType { get; set; } = null;
    public override bool Value { get; set; }
    public override bool IsWriteOnly { get; } = true;

    public override string Serialize()
    {
      return "S=" + (Value ? "1" : "0") + "\r\n";
    }

    public override void Deserialize(string val)
    {

    }

    public override Task Execute(string[] args)
    {

      Value = _trueEntries.Contains(args[0].Trim().ToLower());
      var sampleDevice = ServiceLocator.Current.GetInstance<ISampleDevice>();
      sampleDevice.SetBool(Value);
      return Task.CompletedTask;

    }

    public override string HelpString { get; } = "Usage: Provide a boolean for the sample device.\n" +
                                                "Ex: SAMPLE_SETBOOL TRUE (Set the value to to true)." +
                                                "Ex. SAMPLE_SETBOOL FALSE (Set the value to false).";

    public override bool Validate(string[] args)
    {
      if (args.Length != ArgCount)
        return false;

      return _validEntries.Contains(args[0].ToLower());
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(ISampleDevice);
  }
}
