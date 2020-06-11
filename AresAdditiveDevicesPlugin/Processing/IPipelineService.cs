using System;
using System.Collections.Generic;
using AresAdditiveDevicesPlugin.Processing.Staging;
using DynamicData;

namespace AresAdditiveDevicesPlugin.Processing
{
    public interface IPipelineService
    {
        IObservableCache<IStageEntry, Guid> AllStageEntries { get; }
        void Add(IStageEntry stageEntry);
        void Clear();
        void RemoveEntry(IStageEntry stageEntry);
        void GrabEntry(IStageEntry stageEntry);

        IEnumerable<Guid> StageEntryIdsDependentTo(Guid targetId);
    }
}
