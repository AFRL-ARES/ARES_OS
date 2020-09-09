using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using AresCNTDevicesPlugin.Laser.Config;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresCNTDevicesPlugin.Laser.UI.Config
{
  public class LaserAppConfigViewModel: BasicReactiveObjectDisposable
  {
    private ILaserAppConfig _appConfig;

    public LaserAppConfigViewModel(IVerdiV6Laser laser, ILaserAppConfig appConfig)
    {
      _appConfig = appConfig;
      CommitValuesCommand = ReactiveCommand.Create<Unit>( t =>
      {
          laser.Close( AppConfig );
          laser.Activate();
        AppConfig.Save();
      } );
    }

    public ILaserAppConfig AppConfig
    {
      get => _appConfig;
      set => this.RaiseAndSetIfChanged(ref _appConfig, value);
    }

    public ReactiveCommand<Unit,Unit> CommitValuesCommand { get; set; }
  }
}
