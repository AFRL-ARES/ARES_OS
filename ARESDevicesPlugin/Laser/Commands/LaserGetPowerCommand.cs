using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.ErrorSupport.Impl;
using ARESCore.Util;
using CommonServiceLocator;

namespace AresCNTDevicesPlugin.Laser.Commands
{
   public class LaserGetPowerCommand : AresDeviceCommand<double>
   {
      private double _value;
      public override string ScriptName { get; } = "LASER_GETPOWER";
      public override int ArgCount { get; } = 0;
      public override ConstrainedValue<double> Constraints { get; set; } = new LaserPowerConstrainedValue();
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
         var laser = ServiceLocator.Current.GetInstance<IVerdiV6Laser>();

         try
         {
            var power = laser.Power;
            Value = power;
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
