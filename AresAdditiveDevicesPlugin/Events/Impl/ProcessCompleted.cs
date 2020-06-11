using System;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class ProcessCompleted : IEventAction
    {
        public ProcessCompleted(int id, Type type)
        {
            ProcessId = id;

            // All processes are singleton, allowing lookup by type
            ProcessType = type;
        }

        public int ProcessId { get; set; }
        public Type ProcessType { get; set; }
    }
}
