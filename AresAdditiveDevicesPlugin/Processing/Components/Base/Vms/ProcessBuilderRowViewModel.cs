using System.Reactive;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Processing.Staging;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base.Vms
{
    public class ProcessBuilderRowViewModel : BasicReactiveObjectDisposable
    {
        private IStageEntry _stageEntry;
        private bool _isValid = true;
        private readonly EventHub _eventHub;


        public ProcessBuilderRowViewModel(
            IStageEntry stageEntry,
            EventHub eventHub,
            InputEditMonitor inputEditMonitor,
            bool isValid)
        {
            StageEntry = stageEntry;
            _eventHub = eventHub;
            IsValid = isValid;
            EditStageEntry = ReactiveCommand.CreateFromTask(async _ => await OnEdit());
            DeleteStageEntry = ReactiveCommand.CreateFromTask(async () => await eventHub.Publish(new RemoveStageEntry(stageEntry)));
            TestComponentExecution = ReactiveCommand.CreateFromTask(ExecuteComponent);

            Disposables.Add(EditStageEntry);
            Disposables.Add(DeleteStageEntry);
            Disposables.Add(TestComponentExecution);
        }

        private async Task ExecuteComponent()
        {
            await _stageEntry.Process.StartComponent(StageEntry.Inputs);
        }

        private async Task OnEdit()
        {
            EditStageEntry editEvent = new EditStageEntry(StageEntry);
            await _eventHub.Publish(editEvent);
        }

        public override string ToString()
        {
            return StageEntry.ToString();
        }

        public IStageEntry StageEntry
        {
            get => _stageEntry;
            set => this.RaiseAndSetIfChanged(ref _stageEntry, value);
        }

        public bool IsValid
        {
            get => _isValid;
            set => this.RaiseAndSetIfChanged(ref _isValid, value);
        }

        public ReactiveCommand<Unit, Unit> DeleteStageEntry { get; set; }
        public ReactiveCommand<Unit, Unit> EditStageEntry { get; }
        public ReactiveCommand<Unit, Unit> TestComponentExecution { get; }
    }
}
