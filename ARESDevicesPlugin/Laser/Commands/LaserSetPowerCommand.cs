using System;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.ErrorSupport.Impl;
using ARESCore.Util;
using CommonServiceLocator;

namespace AresCNTDevicesPlugin.Laser.Commands
{
   public class LaserSetPowerCommand : AresDeviceCommand<double>
   {
      public override string ScriptName { get; } = "LASER_POWER";
      public override int ArgCount { get; } = 1;
      public override ConstrainedValue<double> Constraints { get; set; } = new LaserPowerConstrainedValue();
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
         Error = new AresError { Severity = ErrorSeverity.Error, Text = "Step Handling Command" };
         var x = double.Parse(args[0]);

         Value = Constraints.Constrain(x);

         var laser = ServiceLocator.Current.GetInstance<IVerdiV6Laser>();

         try
         {
            laser.SetPower(Value);
         }
         catch (Exception e)
         {
            Error = new AresError() { Severity = ErrorSeverity.Error, Text = "Failed to set the laser power. Cannot perform experiment at the current location." + e.ToString() };
            throw;
         }

         return Task.CompletedTask;

      }

      public override string HelpString { get; } = "Usage: Provide a single double representing the desired Laser Power.\n" +
                                                 "Ex: LASER_POWER 1.2 (Set the Laser Power to 1.2 Watts)." +
                                                 "Ex. LASER_POWER VAL_LASER (Set the Laser Power to the experiment's desired value).";

      public override bool Validate(string[] args)
      {
         if (args.Length != ArgCount)
            return false;
         double val;
         if (double.TryParse(args[0], out val))
            return true;
         if (args[0].Equals(PlanValueString))
            return true;
         return false;
      }

      public override Type AssociatedDeviceType { get; set; } = typeof(IVerdiV6Laser);

      public override bool IsPlannable { get; set; } = true;

      public override string PlanValueString { get; set; } = "VAL_LASER";
   }
}
