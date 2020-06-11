using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Json;
using ARESCore.DisposePatternHelpers;
using DynamicData.Binding;
using Newtonsoft.Json;

namespace AresAdditiveDevicesPlugin.Processing.Staging
{
  [JsonConverter(typeof(StageEntryJsonConverter))]
  public interface IStageEntry : IBasicReactiveObjectDisposable
  {
    int StageId { get; set; }
    Guid UniqueId { get; set; }
    IComponent Process { get; set; }
    IObservableCollection<IProcessData> Inputs { get; set; }
    StageEntryInputMappings InputMappings { get; set; }

    Task ExecuteProcess();
  }
}
