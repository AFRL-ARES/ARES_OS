using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.ErrorSupport.Impl;
using CommonServiceLocator;

namespace AresCNTDevicesPlugin.Laser.Commands
{
  public class LaserSetShutterCommand : AresDeviceCommand<bool>
  {
    private readonly List<string> _trueEntries = new List<string>() { "true", "on", "open", "yes", "1" };
    private readonly List<string> _validEntries = new List<string>() { "true", "on", "open", "yes", "1", "false", "off", "closed", "no", "0" };
    public override string ScriptName { get; } = "LASER_SHUTTER";
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
      var laser = ServiceLocator.Current.GetInstance<IVerdiV6Laser>();

      try
      {
         laser.SetShutter(Value);
      }
      catch (Exception e)
      {
         Error = new AresError() { Severity = ErrorSeverity.Error, Text = "Failed to Set Shutter. Cannot perform experiment at the current location." + e.ToString() };
         throw;
      }

      return Task.CompletedTask;

    }

    public override string HelpString { get; } = "Usage: Provide a boolean for the Laser shutter.\n" +
                                                "Ex: LASER_SHUTTER TRUE (Set the Laser Shutter to Open)." +
                                                "Ex. LASER_SHUTTER FALSE (Set the Laser Shutter to Closed).";

    public override bool Validate(string[] args)
    {
      if (args.Length != ArgCount)
        return false;

      return _validEntries.Contains(args[0].ToLower());
    }

    public override Type AssociatedDeviceType { get; set; } = typeof(IVerdiV6Laser);
  }
}
