using ARESCore.DisposePatternHelpers;
using ARESDevicesPlugin.Laser.Config;
using ReactiveUI;
using System.Reactive;

namespace ARESDevicesPlugin.Laser.UI.Config
{
  public class LaserAppConfigViewModel : BasicReactiveObjectDisposable
  {
    private ILaserAppConfig _appConfig;

    public LaserAppConfigViewModel(IVerdiV6Laser laser, ILaserAppConfig appConfig)
    {
      _appConfig = appConfig;
      CommitValuesCommand = ReactiveCommand.Create<Unit>(t =>
     {
       laser.Close(AppConfig);
       laser.Activate();
       AppConfig.Save();
     });
    }

    public ILaserAppConfig AppConfig
    {
      get => _appConfig;
      set => this.RaiseAndSetIfChanged(ref _appConfig, value);
    }

    public ReactiveCommand<Unit, Unit> CommitValuesCommand { get; set; }
  }
}
