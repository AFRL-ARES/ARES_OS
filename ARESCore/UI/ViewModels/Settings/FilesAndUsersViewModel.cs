using System.Reactive;
using ARESCore.Configurations;
using ARESCore.DisposePatternHelpers;
using ARESCore.Registries;
using ARESCore.UserSession;
using ReactiveUI;

namespace ARESCore.UI.ViewModels.Settings
{
  public class FilesAndUsersViewModel : BasicReactiveObjectDisposable
  {
    private IApplicationConfiguration _appConfig;
    private  ICurrentConfig _currConfig;
    private IConfigManagerRegistry _registry;
    private IUserInfo _selectedUser;

    public FilesAndUsersViewModel(ICurrentConfig currConfig, IApplicationConfiguration appConfig )
    {
      AppConfig = appConfig;
      CurrConfig = currConfig;
      DeleteUserCommand = ReactiveCommand.Create<IUserInfo>( i => Removeit( i ) );
    }

    private void Removeit( IUserInfo userInfo )
    {
      AppConfig.RemoveUser( userInfo );
    }

    public IConfigManagerRegistry Registry
    {
      get => _registry;
      set => this.RaiseAndSetIfChanged( ref _registry, value );
    }

    public ICurrentConfig CurrConfig
    {
      get => _currConfig;
      set => this.RaiseAndSetIfChanged( ref _currConfig, value);
    }

    public IApplicationConfiguration AppConfig
    {
      get => _appConfig;
      set => this.RaiseAndSetIfChanged( ref _appConfig, value);
    }

    public IUserInfo SelectedUser
    {
      get => _selectedUser;
      set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
    }

    public ReactiveCommand<IUserInfo, Unit> DeleteUserCommand { get; set; }
  }
}