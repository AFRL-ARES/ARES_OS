using AresAdditiveDevicesPlugin.Processing.Components.Base.Vms;

namespace AresAdditiveDevicesPlugin.Processing
{
    public interface IProcessDataInputRowViewModelFactory
    {
        ProcessDataInputRowViewModel Create(IProcessData input);
    }
}
