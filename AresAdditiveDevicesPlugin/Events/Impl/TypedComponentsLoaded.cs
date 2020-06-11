using System;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class TypedComponentsLoaded : IEventAction
    {
        public TypedComponentsLoaded(Type componentType)
        {
            ComponentType = componentType;
        }

        public Type ComponentType { get; set; }
    }
}
