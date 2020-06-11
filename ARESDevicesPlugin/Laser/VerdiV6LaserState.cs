using ARESCore.DeviceSupport;
using ReactiveUI;

namespace ARESDevicesPlugin.Laser
{
  public class VerdiV6LaserState : AresDeviceState
  {
    private double _laserPower = 0.01;
    private bool _laserShutter;
    private LaserPowerConstrainedValue _powerLimits = new LaserPowerConstrainedValue();

    public LaserPowerConstrainedValue PowerLimits
    {
      get { return _powerLimits; }
      set { this.RaiseAndSetIfChanged(ref _powerLimits, value); }
    }

    public double LaserPower
    {
      get => _laserPower;
      set
      {
        if (value >= _powerLimits.MinValue && value <= _powerLimits.MaxValue)
          this.RaiseAndSetIfChanged(ref _laserPower, value);
      }
    }

    public bool LaserShutter
    {
      get => _laserShutter;
      set => this.RaiseAndSetIfChanged(ref _laserShutter, value);
    }

    public VerdiV6LaserState()
    {
      LaserShutter = false;
      LaserPower = 0.0;
    }
  }
}