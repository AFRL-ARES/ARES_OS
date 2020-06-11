namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class OpenComponentEditor : IEventAction
    {
        public OpenComponentEditor(bool isOpen)
        {
            IsOpen = isOpen;
        }

        public bool IsOpen { get; }
    }
}
