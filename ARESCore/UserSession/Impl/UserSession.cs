using System;
using System.IO;
using System.Reflection;
using ARESCore.DisposePatternHelpers;
using ARESCore.UI;
using Newtonsoft.Json;
using Ninject;
using ReactiveUI;

namespace ARESCore.UserSession.Impl
{
  internal class UserSession: BasicReactiveObjectDisposable, IUserSession
  {
    private string _username = "DefaultUserName";
    private string _saveDirectory;
    private string _fileNameAndPath;

    [JsonProperty]
    public string Username
    {
      get => _username;
      set => this.RaiseAndSetIfChanged(ref _username , value);
    }

    [JsonProperty]
    public string SaveDirectory
    {
      get => _saveDirectory;
      set => this.RaiseAndSetIfChanged(ref _saveDirectory, value);
    }

    [JsonProperty]
    public string FileNameAndPath
    {
      get => _fileNameAndPath;
      set => this.RaiseAndSetIfChanged(ref _fileNameAndPath, value);
    }
    
    public UserSession()
      {
        SaveDirectory = Path.GetDirectoryName( Assembly.GetExecutingAssembly().CodeBase ).Replace( "file:\\", "" ) + "\\" + Username + "\\";
        FileNameAndPath = SaveDirectory + Username + "_UserSession.json";
      }

    public bool SaveSession()
      {
        try
        {
          string sessionJson = JsonConvert.SerializeObject( this, Formatting.Indented );
          File.WriteAllText( this.FileNameAndPath, sessionJson );
        }
        catch ( Exception ex )
        {
          AresKernel._kernel.Get<IAresConsole>().WriteLine( ex.Message );
          return false;
        }

        return true;
      }
    }
  }
