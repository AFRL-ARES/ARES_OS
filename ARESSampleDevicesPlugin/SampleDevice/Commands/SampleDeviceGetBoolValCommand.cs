using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresSampleDevicesPlugin.SampleDevice.Commands
{
  public class SampleDeviceGetBoolValCommand : AresDeviceCommand<bool>
  {
    public override string ScriptName { get; } = "SAMPLE_GETBOOL";
    public override int ArgCount { get; } = 0;
    public override ConstrainedValue<bool> Constraints { get; set; } = null;
    public override bool Value { get; set; }
    public override Enum UnitType { get; set; } = null;
    public override bool IsWriteOnly { get; } = false;
    public override string Serialize()
    {
      return "?S\r\n";
    }

    public override void Deserialize(string val)
    {
      Value = val.Contains("1");
    }

    public override Task Execute(string[] lines)
    {
      var sampleDevice = ServiceLocator.Current.GetInstance<ISampleDevice>();
      sampleDevice.GetBool();
      Value = sampleDevice.BoolValue;

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
  }
}
