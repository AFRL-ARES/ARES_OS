using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Processing.Staging;
using AresAdditiveDevicesPlugin.Processing.Staging.Impl;
using ARESCore.DisposePatternHelpers;
using ARESCore.Extensions;
using DynamicData;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base.Vms
{
    public class ComponentEditorViewModel : BasicReactiveObjectDisposable
    {
        private readonly EventHub _eventHub;
        private IStageEntry _selectedStageEntry;
        private IStageEntry _editStageEntry;
        private bool _isComponentEditorOpen;
        private bool _editingComponent;
        private readonly IPipelineService _pipelineService;
        private bool _editsApplied;

        public ComponentEditorViewModel(
            EventHub eventHub,
            IObservable<EditStageEntry> editComponentObservable,
            IObservable<OpenComponentEditor> openComponentEditorObservable,
            ComponentSelectorViewModel componentSelectorVm,
            IPipelineService pipelineService)
        {
            _pipelineService = pipelineService;
            _eventHub = eventHub;

            IDisposable editComponentWatcher = editComponentObservable.Subscribe(OnEditEvent);
            IDisposable openComponentEditorWatcher = openComponentEditorObservable.Subscribe(OnOpenEvent);

            IDisposable processesInPipelineWatcher =
                pipelineService
                    .AllStageEntries
                    .Connect()
                    .ObserveOn(DispatcherScheduler.Current)
                    .Bind(out ReadOnlyObservableCollection<IStageEntry> existingStageEntries)
                    .Subscribe();

            AvailableStageEntries = existingStageEntries;

            Cancel = ReactiveCommand.CreateFromTask(async _ => await OnCancel());
            UpdateStageEntry = ReactiveCommand.CreateFromTask(async _ => await OnUpdate());

            var componentSelectedListener = componentSelectorVm.Subscribe(
                componentSelector => componentSelector.SelectedComponent,
                componentSelector => OnComponentSelected(componentSelector.SelectedComponent));

            Disposables.Add(componentSelectedListener);
            Disposables.Add(openComponentEditorWatcher);
            Disposables.Add(editComponentWatcher);
            Disposables.Add(Cancel);
            Disposables.Add(UpdateStageEntry);
            Disposables.Add(processesInPipelineWatcher);
        }

        private async Task OnCancel()
        {
            if (_editingComponent)
            {
                // Put it back
                _pipelineService.Add(SelectedStageEntry);
                _editingComponent = false;
            }

            EditsApplied = false;
            await CloseWindow();
        }

        private async Task OnUpdate()
        {

            SelectedStageEntry.UniqueId = EditStageEntry.UniqueId;
            if (SelectedStageEntry.Process != EditStageEntry.Process)
            {
                SelectedStageEntry.Process = EditStageEntry.Process;
            }

            EditsApplied = true;
            _pipelineService.Add(SelectedStageEntry);

            await CloseWindow();
        }

        private void OnOpenEvent(OpenComponentEditor @event)
        {
            {
                if (@event.IsOpen == true)
                    IsComponentEditorOpen = true;
                else if (@event.IsOpen == false)
                {
                    IsComponentEditorOpen = false;
                }
            }
        }

        private async void OnEditEvent(EditStageEntry editEvent)
        {
            await _eventHub.Publish(new OpenComponentSelector(false));

            IsComponentEditorOpen = true;
            IStageEntry stageEntry = editEvent.StageEntry;

            SelectedStageEntry = stageEntry;
            if (stageEntry.UniqueId == Guid.Empty)
                return;
            _pipelineService.GrabEntry(editEvent.StageEntry);
            _editingComponent = true;
        }

        private async Task CloseWindow()
        {
            await _eventHub.Publish(new OpenComponentEditor(false));
            if (EditStageEntry != null)
            {
                EditStageEntry = null;
                SelectedStageEntry = null;
            }
        }

        private void OnComponentSelected(IComponent selectedComponent)
        {
            if (SelectedStageEntry == null)
            {
                SelectedStageEntry = new StageEntry(selectedComponent) { UniqueId = Guid.NewGuid() };
            }
        }

        public ReactiveCommand<Unit, Unit> UpdateStageEntry { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReadOnlyObservableCollection<IComponent> AvailableComponents { get; set; }
        public ReadOnlyObservableCollection<IStageEntry> AvailableStageEntries { get; set; }

        public IStageEntry SelectedStageEntry
        {
            get => _selectedStageEntry;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedStageEntry, value);

                if (value == null)
                {
                    return;
                }
                if (EditStageEntry == null)
                {
                    EditStageEntry = new StageEntry(SelectedStageEntry);
                }
                else
                {
                    if (EditStageEntry?.Process == value.Process)
                        return;

                    EditStageEntry = new StageEntry
                    {
                        Process = value.Process,
                        Inputs = SelectedStageEntry.Process == value.Process
                            ? SelectedStageEntry.Inputs
                            : value.Process.DefaultInputs,
                        UniqueId = SelectedStageEntry.UniqueId
                    };
                }
            }
        }

        public IStageEntry EditStageEntry
        {
            get => _editStageEntry;
            set => this.RaiseAndSetIfChanged(ref _editStageEntry, value);
        }


        public bool IsComponentEditorOpen
        {
            get => _isComponentEditorOpen;
            set => this.RaiseAndSetIfChanged(ref _isComponentEditorOpen, value);
        }

        public bool EditsApplied
        {
            get => _editsApplied;

            set
            {
                _editsApplied = value;
                this.RaisePropertyChanged(); // Force notification
            }
        }
    }
}
