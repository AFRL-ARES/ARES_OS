using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using ARESCore.DisposePatternHelpers;
using ARESCore.Util;
using Newtonsoft.Json;
using ReactiveUI;

namespace ARESCore.UserSession.Impl
{
  internal class UserInfo: BasicReactiveObjectDisposable, IUserInfo
  {
    
    private string _username;
    private string _saveFileName;
    private string _saveDirectory;
    private bool _licenseAccepted;
    private DateTime _lastLoadedDate;

    [JsonProperty]
    public string Username
    {
      get => _username;
      set => this.RaiseAndSetIfChanged(ref _username, value);
    }
    [JsonProperty]
    public string SaveFileName
    {
      get => _saveFileName;
      set => this.RaiseAndSetIfChanged(ref _saveFileName, value);
    }
    [JsonProperty]
    public string SaveDirectory
    {
      get => _saveDirectory;
      set => this.RaiseAndSetIfChanged(ref _saveDirectory , value);
    }
    [JsonProperty]
    public DateTime LastLoadedDate
    {
      get => _lastLoadedDate;
      set => this.RaiseAndSetIfChanged(ref _lastLoadedDate, value);
    }
  }
}
