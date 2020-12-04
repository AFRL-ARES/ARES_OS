using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using ARESCore.DisposePatternHelpers;
using ARESCore.UI;
using ARESCore.UserSession;
using ARESCore.UserSession.Impl;
using ARESCore.Util;
using Newtonsoft.Json;
using Ninject;
using ReactiveUI;

namespace ARESCore.Configurations.impl
{
  /// <summary>
  /// ApplicationConfiguration
  /// </summary>
  public class ApplicationConfiguration : BasicReactiveObjectDisposable, IApplicationConfiguration
  {
    private string _applicationRootDirectory;
    private string _currentAppConfigPath;
    private ObservableCollection<IUserInfo> _userList = new ObservableCollection<IUserInfo>();
    private List<IProjectInfo> _projectList = new List<IProjectInfo>();

    public void SetDefaults()
    {
      ApplicationRootDirectory = $@"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.Parent?.FullName}\ConfigFiles\";
      CurrentAppConfigPath = ApplicationRootDirectory + "ARESConfig.json";
    }

    public void LoadConfig( string configPath = "" )
    {
      if ( configPath.Equals( "" ) )
      {
        string rootDir = $@"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.Parent?.FullName}\ConfigFiles\";
        configPath = rootDir + "ARESConfig.json";
        SetDefaults();
      }

      if ( File.Exists( configPath ) )
      {
        try
        {
          string configJson = File.ReadAllText( configPath );
          var appConfig = JsonConvert.DeserializeObject<ApplicationConfiguration>( configJson );
          CopyFields( appConfig );
          CurrentAppConfigPath = configPath;
        }
        catch ( Exception ex )
        {
          AresKernel._kernel.Get<IAresConsole>().WriteLine( ex.Message );
        }
      }
    }

    private void CopyFields( IApplicationConfiguration appConfig )
    {
      ApplicationRootDirectory = $@"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.Parent?.FullName}\ConfigFiles\";
      this.ProjectList = appConfig.ProjectList;
      this.UserList = appConfig.UserList;
    }

    public bool SaveConfig( string configPath = "" )
    {
      if ( configPath.Equals( "" ) )
      {
        string rootDir = $@"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.Parent?.FullName}\ConfigFiles\";
        configPath = rootDir + "ARESConfig.json";
      }

      try
      {
        string configJson = JsonConvert.SerializeObject
        (
          this, Formatting.Indented, new JsonSerializerSettings()
          {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
          }
        );
        File.WriteAllText( configPath, configJson );
      }
      catch ( Exception ex )
      {
        AresKernel._kernel.Get<IAresConsole>().WriteLine( ex.Message );
        return false;
      }

      return true;
    }

    [JsonProperty]
    public string ApplicationRootDirectory
    {
      get => _applicationRootDirectory;
      set => this.RaiseAndSetIfChanged( ref _applicationRootDirectory, value );
    }

    [JsonProperty]
    public string CurrentAppConfigPath
    {
      get => _currentAppConfigPath;
      set => this.RaiseAndSetIfChanged( ref _currentAppConfigPath, value );
    }

    [JsonProperty( ItemConverterType = typeof( JsonTypeConverter<UserInfo> ) )]
    public ObservableCollection<IUserInfo> UserList
    {
      get => _userList;
      set => this.RaiseAndSetIfChanged( ref _userList, value );
    }

    [JsonProperty( ItemConverterType = typeof( JsonTypeConverter<ProjectInfo> ) )]
    public List<IProjectInfo> ProjectList
    {
      get => _projectList;
      set => this.RaiseAndSetIfChanged( ref _projectList, value );
    }

    public void RemoveUser( IUserInfo userInfo )
    {
      UserList.Remove( userInfo );
      if (File.Exists(userInfo.SaveFileName))
        File.Delete( userInfo.SaveFileName );
      SaveConfig();
    }
  }
}