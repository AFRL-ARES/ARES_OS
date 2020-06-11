using ARESCore.Configurations;
using ARESCore.DisposePatternHelpers;
using ARESCore.UI;
using CommonServiceLocator;
using Ninject;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows;

namespace ARESCore.UserSession
{
  public class UserSessionSelectionViewModel : BasicReactiveObjectDisposable
  {
    private string _appConfig = "";
    private int _sessionSelection;
    private string _workingDirectory = "";
    private string _newUserName = "";
    private bool? _dialogResult;
    private bool _userIsSelected;

    public UserSessionSelectionViewModel()
    {
      LocalUserList = new ObservableCollection<IUserInfo>();
      NotificationRequest = new InteractionRequest<INotification>();
      LoadSessionCommand = ReactiveCommand.Create(() => Application.Current.Dispatcher.Invoke(LoadSession));
      CreateNewSessionCommand = ReactiveCommand.Create(() => ServiceLocator.Current.GetInstance<UserSessionSelection>().Dispatcher.Invoke(CreateNewSession));
      LocalUserNames = new ObservableCollection<string>();
      IApplicationConfiguration loadedConfig = AresKernel._kernel.Get<IApplicationConfiguration>();

      // Try to load the Application Configuration
      loadedConfig.LoadConfig("");
      AppConfig = loadedConfig.CurrentAppConfigPath;
      if (loadedConfig.UserList != null && loadedConfig.UserList.Count != 0)
      {
        // Get a copy of the user list locally
        LocalUserList = loadedConfig.UserList;
        foreach (var userInfo in LocalUserList)
        {
          LocalUserNames.Add(userInfo.Username);
        }
        WorkingDirectory = LocalUserList[SessionSelection].SaveDirectory;
        UserIsSelected = true;
      }
    }

    public InteractionRequest<INotification> NotificationRequest { get; set; }

    private void CreateNewSession()
    {
      foreach (char illegalChar in System.IO.Path.GetInvalidFileNameChars())
        NewUserName = NewUserName.Replace(illegalChar.ToString(), "");

      // Create the file path and name for the user session JSON (Use the text of the selected working directory)
      string saveFilePath = WorkingDirectory + "\\" + NewUserName + "_UserSession.json";

      // Make sure the user name is atleast something that can be made into a filename
      if (NewUserName.Length == 0)
      {
        var notif = new AresNotification
        {
          Title = "Alert!",
          Content = "Please enter a better username!"
        };
        NotificationRequest.Raise(notif, r => r.Title = "Notification");
        return;
      }

      // Make sure that a workign directory has been selected
      if (WorkingDirectory.Length == 0)
      {
        NotificationRequest.Raise(new AresNotification { Title = "Alert!", Content = "Please select a working directory!" }, r => r.Title = "Notification");
        return;
      }

      // Make sure this username does not already exist
      foreach (var aUser in AresKernel._kernel.Get<IApplicationConfiguration>().UserList)
      {
        if (aUser.Username.Equals(NewUserName))
        {
          NotificationRequest.Raise(new AresNotification { Title = "Alert!", Content = "A user with this name already exists. Please choose a new name." }, r => r.Title = "Notification");
          return;
        }
      }
      IUserInfo info = AresKernel._kernel.Get<IUserInfo>();
      info.SaveFileName = saveFilePath;
      info.Username = NewUserName;
      info.LastLoadedDate = DateTime.Now;
      info.SaveDirectory = WorkingDirectory;
      AresKernel._kernel.Get<ICurrentConfig>().User = info;
      Application.Current.Dispatcher.Invoke(() => DialogResult = true);
    }

    private void LoadSession()
    {
      var user = LocalUserList[SessionSelection];
      user.LastLoadedDate = DateTime.Now;
      AresKernel._kernel.Get<ICurrentConfig>().User = user;
      DialogResult = true;
    }

    public ObservableCollection<IUserInfo> LocalUserList { get; set; }

    public ObservableCollection<string> LocalUserNames { get; set; }

    public string AppConfig
    {
      get => _appConfig;
      set => this.RaiseAndSetIfChanged(ref _appConfig, value);
    }

    public int SessionSelection
    {
      get => _sessionSelection;
      set
      {
        this.RaiseAndSetIfChanged(ref _sessionSelection, value);
        WorkingDirectory = LocalUserList[SessionSelection].SaveDirectory;
      }
    }

    public string WorkingDirectory
    {
      get => _workingDirectory;
      set => this.RaiseAndSetIfChanged(ref _workingDirectory, value);
    }

    public bool UserIsSelected
    {
      get => _userIsSelected;
      set => this.RaiseAndSetIfChanged(ref _userIsSelected, value);
    }

    public string NewUserName
    {
      get => _newUserName;
      set => this.RaiseAndSetIfChanged(ref _newUserName, value);
    }

    public bool? DialogResult
    {
      get => _dialogResult;
      set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
    }

    public ReactiveCommand<Unit, Unit> LoadSessionCommand { get; set; }

    public ReactiveCommand<Unit, Unit> CreateNewSessionCommand { get; set; }
  }
}
