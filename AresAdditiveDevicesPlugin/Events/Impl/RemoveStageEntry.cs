using AresAdditiveDevicesPlugin.Processing.Staging;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class RemoveStageEntry : IEventAction
    {
        public RemoveStageEntry(IStageEntry stageEntry)
        {
            StageEntry = stageEntry;
        }

        public IStageEntry StageEntry { get; set; }
    }
}
