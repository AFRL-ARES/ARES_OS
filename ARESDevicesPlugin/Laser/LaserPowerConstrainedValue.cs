using ARESCore.DeviceSupport;

namespace AresCNTDevicesPlugin.Laser
{
  public class LaserPowerConstrainedValue: ConstrainedValue<double>
  {
    public LaserPowerConstrainedValue()
    {
      MaxValue = 6.0;
      MinValue = 0.01;
    }
  }
}
