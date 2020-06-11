using AresAdditiveDevicesPlugin.Processing.Components.Base.Vms;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
    public class InputVmRepoEdited : IEventAction
    {
        public InputVmRepoEdited(InputViewModelRepo inputVmRepo)
        {
            InputVmRepo = inputVmRepo;
        }

        public InputViewModelRepo InputVmRepo { get; set; }
    }
}
