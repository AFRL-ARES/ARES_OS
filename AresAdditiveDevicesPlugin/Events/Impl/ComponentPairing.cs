using System;
using AresAdditiveDevicesPlugin.Processing;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class ComponentPairing
    {
        public ComponentPairing(int id, Type type)
        {
            ProcessId = id;

            // All processes are singleton, allowing lookup by type
            ProcessType = type;
        }

        public ComponentPairing(IComponent component)
        {
            ProcessId = component.Id;
            ProcessType = component.GetType();
        }

        public override string ToString()
        {
            return ProcessType.Name + " " + ProcessId;
        }

        public int ProcessId { get; set; }
        public Type ProcessType { get; set; }
    }
}
