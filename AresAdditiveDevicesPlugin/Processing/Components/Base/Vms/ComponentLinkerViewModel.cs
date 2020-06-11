using System;
using System.Reactive;
using AresAdditiveDevicesPlugin.Processing.Staging;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using DynamicData;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base.Vms
{
    public class ComponentLinkerViewModel : BasicReactiveObjectDisposable
    {
        private IProcessData _editData;
        private IStageEntry _linkedStageEntry;
        private int _linkedInputIndex = -1;

        public ComponentLinkerViewModel(
            IProcessData editingData)
        {
            EditData = editingData;
            LinkInputCommand = ReactiveCommand.Create(LinkInput);
        }

        private void LinkInput()
        {
            if (LinkedStageEntry != null && LinkedInputIndex >= 0)
            {
                EditData = LinkedStageEntry.Inputs[LinkedInputIndex];
            }
        }

        public IProcessData EditData
        {
            get => _editData;
            set => this.RaiseAndSetIfChanged(ref _editData, value);
        }

        public IStageEntry LinkedStageEntry
        {
            get => _linkedStageEntry;
            set => this.RaiseAndSetIfChanged(ref _linkedStageEntry, value);
        }

        public int LinkedInputIndex
        {
            get => _linkedInputIndex;
            set => this.RaiseAndSetIfChanged(ref _linkedInputIndex, value);
        }

        public IObservableCache<IStageEntry, Guid> AvailableStageEntries => ServiceLocator.Current.GetInstance<IPipelineService>().AllStageEntries;
        public ReactiveCommand<Unit, Unit> LinkInputCommand { get; set; }
    }
}
