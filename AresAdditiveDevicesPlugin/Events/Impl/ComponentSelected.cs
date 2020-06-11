using AresAdditiveDevicesPlugin.Processing;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class ComponentSelected : IEventAction
    {
        public ComponentSelected(IComponent component)
        {
            Component = component;
        }

        public IComponent Component { get; set; }
    }
}
