using AresAdditiveDevicesPlugin.Processing.Staging;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class EditStageEntry : IEventAction
    {
        public EditStageEntry(IStageEntry stageEntry)
        {
            StageEntry = stageEntry;
        }

        public IStageEntry StageEntry { get; set; }
    }
}
