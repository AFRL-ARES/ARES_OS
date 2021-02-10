using System.Reactive;
using System.Windows;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.Configurations.impl
{
  public abstract class ConfigManager : ReactiveSubscriber, IConfigManager
  {
    private FrameworkElement _appConfigView;
    private FrameworkElement _userConfigView;
    private IUserDeviceConfig _userDeviceConfig;
    private IAppDeviceConfig _appConfig;
    private string _deviceName;

    public ConfigManager()
    {
      SaveUserConfigCommand = ReactiveCommand.Create<Unit>( u => SaveUserConfig() );
    }

    public FrameworkElement AppConfigView
    {
      get => _appConfigView;
      set => this.RaiseAndSetIfChanged( ref _appConfigView, value );
    }

    public FrameworkElement UserConfigView
    {
      get => _userConfigView;
      set => this.RaiseAndSetIfChanged( ref _userConfigView, value );
    }

    public IUserDeviceConfig UserDeviceConfig
    {
      get => _userDeviceConfig;
      set => this.RaiseAndSetIfChanged( ref _userDeviceConfig, value );
    }

    public IAppDeviceConfig AppConfig
    {
      get => _appConfig;
      set => this.RaiseAndSetIfChanged( ref _appConfig, value );
    }

    public string DeviceName
    {
      get => _deviceName;
      set => this.RaiseAndSetIfChanged( ref _deviceName, value );
    }

    public ReactiveCommand<Unit, Unit> SaveUserConfigCommand { get; set; }


    public void LoadConfigs()
    {
      UserDeviceConfig?.Load();
      AppConfig?.Load();
    }

    public void SaveUserConfig()
    {
      UserDeviceConfig.Save();
    }
  }
}
