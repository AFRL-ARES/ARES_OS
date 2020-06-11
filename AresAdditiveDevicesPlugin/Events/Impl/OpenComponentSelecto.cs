namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class OpenComponentSelector : IEventAction
    {
        public OpenComponentSelector(bool isOpen)
        {
            IsOpen = isOpen;
        }

        public bool IsOpen { get; }
    }
}
