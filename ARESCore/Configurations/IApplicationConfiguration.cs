using System.Collections.Generic;
using System.Collections.ObjectModel;
using ARESCore.DisposePatternHelpers;
using ARESCore.UserSession;

namespace ARESCore.Configurations
{
  public interface IApplicationConfiguration : IBasicReactiveObjectDisposable
  {
    void SetDefaults();
    void LoadConfig(string configPath);
    bool SaveConfig(string configPath);
    string ApplicationRootDirectory { get; set; }
    string CurrentAppConfigPath { get; set; }
    ObservableCollection<IUserInfo> UserList { get; set; }
    List<IProjectInfo> ProjectList { get; set; }
    void RemoveUser( IUserInfo userInfo );
  }
}