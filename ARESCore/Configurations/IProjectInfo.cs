using System;
using ARESCore.DisposePatternHelpers;
using Newtonsoft.Json;

namespace ARESCore.Configurations
{
  public interface IProjectInfo: IReactiveSubscriber
  {
    [JsonProperty]
    string Creator { get; set; }
    [JsonProperty]
    string Description { get; set; }
    [JsonProperty]
    DateTime LastLoadedDate { get; set; }
    [JsonProperty]
    string SaveDirectory { get; set; }

    [JsonIgnore]
    string SaveDirectory_Scripts { get; }
    [JsonIgnore]
    string SaveDirectory_ExportedManualData { get; }
    [JsonIgnore]
    string SaveDirectory_ExportedPlanningDBs { get; }
    [JsonIgnore]
    string SaveDirectory_ExportedBatchData { get; }
    [JsonIgnore]
    string SaveDirectory_ExportedData { get; }
    bool CreateDirectories();
  }
}
