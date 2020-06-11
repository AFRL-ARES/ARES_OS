using System.Reactive;
using System.Windows;
using ReactiveUI;

namespace ARESCore.Configurations
{
  public interface IConfigManager
  {
    FrameworkElement AppConfigView { get; set; }

    FrameworkElement UserConfigView { get; set; }

    IUserDeviceConfig UserDeviceConfig { get; set; }

    IAppDeviceConfig AppConfig { get; set; }

    string DeviceName { get; set; }
    ReactiveCommand<Unit,Unit> SaveUserConfigCommand { get; set; }

    void LoadConfigs();

    void SaveUserConfig();
  }
}
