using System;
using System.Collections.Generic;
using System.Linq;
using AresCNTDevicesPlugin.Laser.Config;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresCNTDevicesPlugin.Laser.UI.Config
{
  public class LaserUserConfigViewModel : BasicReactiveObjectDisposable
  {
    private ILaserUserDeviceConfig _config;

    public LaserUserConfigViewModel( ILaserUserDeviceConfig config )
    {
      _config = config;
    }

    public ILaserUserDeviceConfig ConfigData
    {
      get => _config;
      set => this.RaiseAndSetIfChanged( ref _config, value );
    }
  }
}
