using ARESCore.DisposePatternHelpers;
using ARESDevicesPlugin.Laser.Config;
using ReactiveUI;

namespace ARESDevicesPlugin.Laser.UI.Config
{
  public class LaserUserConfigViewModel : BasicReactiveObjectDisposable
  {
    private ILaserUserDeviceConfig _config;

    public LaserUserConfigViewModel(ILaserUserDeviceConfig config)
    {
      _config = config;
    }

    public ILaserUserDeviceConfig ConfigData
    {
      get => _config;
      set => this.RaiseAndSetIfChanged(ref _config, value);
    }
  }
}
