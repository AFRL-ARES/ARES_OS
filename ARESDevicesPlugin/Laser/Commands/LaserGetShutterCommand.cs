using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.ErrorSupport.Impl;
using CommonServiceLocator;

namespace ARESDevicesPlugin.Laser.Commands
{
   public class LaserGetShutterCommand : AresDeviceCommand<bool>
   {
      public override string ScriptName { get; } = "LASER_GETSHUTTER";
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
         var laser = ServiceLocator.Current.GetInstance<IVerdiV6Laser>();

         try
         {
            Value = laser.Shutter;
         }
         catch (Exception e)
         {
            Error = new AresError() { Severity = ErrorSeverity.Error, Text = "Failed to get power. Cannot perform experiment at the current location." + e.ToString() };
            throw;
         }

         return Task.CompletedTask;
      }

      public override string HelpString { get; } = "";
      public override bool Validate(string[] args)
      {
         if (args.Length != 0)
            return false;
         return true;
      }

      public override Type AssociatedDeviceType { get; set; } = typeof(IVerdiV6Laser);
   }
}
