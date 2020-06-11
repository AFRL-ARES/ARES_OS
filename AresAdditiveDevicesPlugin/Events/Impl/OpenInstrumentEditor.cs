namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class OpenInstrumentEditor : IEventAction
    {
        public OpenInstrumentEditor(bool isOpen)
        {
            IsOpen = isOpen;
        }

        public bool IsOpen { get; }
    }
}
