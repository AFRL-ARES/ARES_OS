using System;
using AresAdditiveDevicesPlugin.Processing.Json;
using ARESCore.DisposePatternHelpers;
using Newtonsoft.Json;

namespace AresAdditiveDevicesPlugin.Processing
{
    [JsonConverter(typeof(ProcessDataJasonConverter))]
    public interface IProcessData : IBasicReactiveObjectDisposable
    {
        object Data { get; set; }
        Type Type { get; set; }
        string Name { get; set; }
        bool RequiresSetup { get; set; }
        bool IsResult { get; set; }
        bool IsAnalysisImage { get; set; }
    }
}
