using System;
using Newtonsoft.Json;

namespace AresAdditiveDevicesPlugin.Processing.Impl
{
    public class StageEntryInputMapping
    {
        [JsonProperty("InputIndex")]
        public int InputIndex { get; set; }

        [JsonProperty("LinkedEntryId")]
        public Guid LinkedEntryId { get; set; }

        [JsonProperty("LinkEntryInputIndex")]
        public int LinkEntryInputIndex { get; set; }
    }
}
