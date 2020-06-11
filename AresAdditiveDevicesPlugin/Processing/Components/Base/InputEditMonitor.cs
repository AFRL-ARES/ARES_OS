using System;
using System.Linq;
using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Processing.Components.Base.Vms;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Staging;
using ARESCore.DisposePatternHelpers;
using ARESCore.Extensions;
using DynamicData;
using MoreLinq;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base
{
    public class InputEditMonitor : BasicReactiveObjectDisposable
    {
        private InputViewModelRepo _inputVms;
        private readonly SourceCache<InputViewModelRepo, Guid> _entryInputVmsRepo = new SourceCache<InputViewModelRepo, Guid>(vmRepo => vmRepo.UniqueStageEntryId);
        private readonly IPipelineService _pipelineService;
        private readonly EventHub _eventHub;
        private IStageEntry _previouslyRemovedStageEntry;

        public InputEditMonitor(
          EventHub eventHub,
          ComponentEditorViewModel componentEditor,
          IPipelineService pipelineService
          )
        {
            _eventHub = eventHub;
            _pipelineService = pipelineService;
            Disposables.Add(componentEditor.Subscribe(compEditor => compEditor.EditsApplied, compEditor => OnEditsApplied(compEditor.SelectedStageEntry, compEditor.EditsApplied)));
            Disposables.Add(componentEditor.Subscribe(compEditor => compEditor.EditStageEntry, compEditor => OnEditEntrySelected(compEditor.EditStageEntry)));
            Disposables.Add(eventHub.GetEvent<RemoveStageEntry>().Subscribe(OnEntryRemoved));
        }

        private void OnEditsApplied(IStageEntry selectedEntry, bool editsApplied)
        {
            if (!editsApplied)
                return;
            selectedEntry.Inputs.Clear();
            selectedEntry.Inputs.AddRange(SelectedInputViewModels.Select(vm => vm.InputEdit));
            selectedEntry.InputMappings.Clear();

            var linkedVms = SelectedInputViewModels.Where(vm => vm.IsLinkedEdit);

            selectedEntry.InputMappings.AddRange(linkedVms.Select(vm =>
            {
                var inputIndex = SelectedInputViewModels.IndexOf(vm);
                if (vm.LinkedEntryEdit == null)
                {
                    return new StageEntryInputMapping { InputIndex = inputIndex, LinkEntryInputIndex = -1 };
                }
                var linkedEntryId = vm.LinkedEntryEdit.UniqueId;
                if (vm.LinkedInputEdit == null)
                {
                    return new StageEntryInputMapping { InputIndex = inputIndex, LinkedEntryId = linkedEntryId, LinkEntryInputIndex = -1 };
                }
                var linkedEntryInputIndex = vm.LinkedEntryEdit.Inputs.IndexOf(vm.LinkedInputEdit);
                return new StageEntryInputMapping { InputIndex = inputIndex, LinkedEntryId = linkedEntryId, LinkEntryInputIndex = linkedEntryInputIndex };
            }));

            _eventHub.Publish(new InputVmRepoEdited(SelectedInputViewModels));
        }

        private void OnEntryRemoved(RemoveStageEntry removeEvent)
        {
            _previouslyRemovedStageEntry = removeEvent.StageEntry;
        }

        private void OnEditEntrySelected(IStageEntry editEntry)
        {
            if (editEntry == null)
            {
                SelectedInputViewModels = null;
                return;
            }

            var inputViewModels = new InputViewModelRepo(editEntry);
            editEntry.InputMappings.ForEach(mapping =>
            {
                var affectedVm = inputViewModels[mapping.InputIndex];
                affectedVm.IsLinked = true;
                var linkedEntryLookup = _pipelineService.AllStageEntries.Lookup(mapping.LinkedEntryId);
                if (linkedEntryLookup.HasValue)
                {
                    affectedVm.LinkedEntry = linkedEntryLookup.Value;

                    if (mapping.LinkEntryInputIndex >= 0)
                    {
                        var linkedInput = linkedEntryLookup.Value.Inputs[mapping.LinkEntryInputIndex];
                        affectedVm.LinkedInput = linkedInput;
                    }
                    else
                    {
                        affectedVm.LinkedInput = null;
                    }
                }
                affectedVm.ResetEdit();
            });
            SelectedInputViewModels = inputViewModels;
        }

        public InputViewModelRepo SelectedInputViewModels
        {
            get => _inputVms;
            set
            {
                if (value == null)
                {
                    this.RaiseAndSetIfChanged(ref _inputVms, new InputViewModelRepo());
                    return;
                }
                this.RaiseAndSetIfChanged(ref _inputVms, value);
            }
        }
    }
}
