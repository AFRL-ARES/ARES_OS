using ARESCore.DeviceSupport;

namespace ARESDevicesPlugin.Laser
{
  public class LaserPowerConstrainedValue : ConstrainedValue<double>
  {
    public LaserPowerConstrainedValue()
    {
      MaxValue = 6.0;
      MinValue = 0.01;
    }
  }
}
