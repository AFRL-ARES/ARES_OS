using System;
using System.IO;
using ARESCore.DisposePatternHelpers;
using Newtonsoft.Json;
using ReactiveUI;

namespace ARESCore.Configurations.impl
{
  internal class ProjectInfo : BasicReactiveObjectDisposable, IProjectInfo
  {
    private string _creator;
    private string _description;
    private DateTime _lastLoadedDate;
    private string _saveDirectory;

    public bool CreateDirectories()
    {
      try
      {
        Directory.CreateDirectory( SaveDirectory_Scripts );
        Directory.CreateDirectory( SaveDirectory_ExportedManualData );
        Directory.CreateDirectory( SaveDirectory_ExportedPlanningDBs );
        Directory.CreateDirectory( SaveDirectory_ExportedBatchData );
        Directory.CreateDirectory( SaveDirectory_ExportedData );
        return true;
      }
      catch ( Exception ) { return false; }
    }

    public string Creator
    {
      get => _creator;
      set => this.RaiseAndSetIfChanged( ref _creator, value );
    }

    public string Description
    {
      get => _description;
      set => this.RaiseAndSetIfChanged( ref _description, value );
    }
    public DateTime LastLoadedDate
    {
      get => _lastLoadedDate;
      set => this.RaiseAndSetIfChanged( ref _lastLoadedDate, value );
    }
    public string SaveDirectory
    {
      get => _saveDirectory;
      set => this.RaiseAndSetIfChanged( ref _saveDirectory, value );
    }

    [JsonIgnore]
    public string SaveDirectory_Scripts { get { return SaveDirectory + "\\Scripts\\"; } }
    [JsonIgnore]
    public string SaveDirectory_ExportedManualData { get { return SaveDirectory + "\\ExportedManualData\\"; } }
    [JsonIgnore]
    public string SaveDirectory_ExportedPlanningDBs { get { return SaveDirectory + "\\ExportedPlanningDatabases\\"; } }
    [JsonIgnore]
    public string SaveDirectory_ExportedBatchData { get { return SaveDirectory + "\\ExportedBatchData\\"; } }
    [JsonIgnore]
    public string SaveDirectory_ExportedData { get { return SaveDirectory + "\\ExportedData\\"; } }
  }
}