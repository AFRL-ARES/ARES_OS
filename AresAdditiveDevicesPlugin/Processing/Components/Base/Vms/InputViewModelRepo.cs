using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Staging;
using ARESCore.Extensions;
using CommonServiceLocator;
using MoreLinq;
using Ninject.Infrastructure.Language;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base.Vms
{
  public class InputViewModelRepo : ObservableCollection<ProcessDataInputRowViewModel>
  {
    private bool _isValid = true;
    private bool _isLinkable = false;
    private readonly SimpleDisposable _disposables = new SimpleDisposable();

    public InputViewModelRepo()
    {
      this.SubscribeAndInvoke(item => _disposables.Add(InputValiditySubscription(item)));
    }

    //    protected void OnItemAdded(ProcessDataInputRowViewModel item)
    //    {
    //      base.OnItemAdded(item);
    //      Disposables.Add(InputValiditySubscription(item));

    //    }

    private IDisposable InputValiditySubscription(ProcessDataInputRowViewModel inputVm)
    {
      return inputVm.SubscribeAndInvoke(pdVm => pdVm.IsValid, pdVm =>
      {
        IsValid = CheckValidity(pdVm);
      });
    }

    private bool CheckValidity(ProcessDataInputRowViewModel inputVm)
    {
      // This one is invalid
      return inputVm.IsValid && this.Where(pdVm => pdVm != inputVm).All(pdVm => pdVm.IsValid);
    }


    public InputViewModelRepo(Guid id)
    {
      UniqueStageEntryId = id;
    }

    public InputViewModelRepo(IStageEntry stageEntry)
    {
      UniqueStageEntryId = stageEntry.UniqueId;
      stageEntry.Inputs
          .Map(input =>
              Add(ServiceLocator.Current.GetInstance<IProcessDataInputRowViewModelFactory>().Create(input)));
      AssignEdits();
    }

    public void AssignEdits()
    {
      // Enumerations hold references without Clone()
      this.Map(vm =>
      {
        var inputType = vm.Input.Type;
        if (vm.Input.Data is Enum)
        {
          inputType = vm.Input.Data.GetType();
        }
        vm.InputEdit =
                  (IProcessData)Activator.CreateInstance(typeof(ProcessData<>).MakeGenericType(inputType), vm.Input);

      });
    }

    public StageEntryInputMappings GetInputMappings()
    {
      StageEntryInputMappings inputMappings = new StageEntryInputMappings();
      this.Where(pdVm => pdVm.IsLinked && pdVm.LinkedInput != null).ForEach(pdVm =>
      {
        var entryId = pdVm.LinkedEntry.UniqueId;
        var linkedInputIndex = pdVm.LinkedEntry.Inputs.IndexOf(pdVm.LinkedInput);
        //        var linkedInputIndex = pdVm.LinkedEntry.Inputs.FindIndex(pd => pd == pdVm.LinkedInput);
        int inputIndex = IndexOf(pdVm);
        StageEntryInputMapping inputMapping = new StageEntryInputMapping
        {
          LinkedEntryId = entryId,
          LinkEntryInputIndex = linkedInputIndex,
          InputIndex = inputIndex
        };
        inputMappings.Add(inputMapping);
      });

      return inputMappings;
    }

    public Guid UniqueStageEntryId { get; set; }

    public bool IsValid
    {
      get => _isValid;
      set
      {
        var changed = value != _isValid;
        _isValid = value;
        if (changed)
        {
          OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
        }
      }
    }

    public bool IsLinkable
    {
      get => _isLinkable;
      set
      {
        var changed = value != _isLinkable;
        _isLinkable = value;
        if (changed)
        {
          OnPropertyChanged(new PropertyChangedEventArgs("IsLinkable"));
        }
      }
    }

    public IList<IStageEntry> LinkableEntries
    {
      get
      {
        var pipelineService = ServiceLocator.Current.GetInstance<IPipelineService>();
        var allStageEntries = pipelineService.AllStageEntries.Items;
        var unlinkableEntries = pipelineService.StageEntryIdsDependentTo(UniqueStageEntryId);
        var linkableEntries = allStageEntries.Where(entry =>
            !unlinkableEntries.Contains(entry.UniqueId) && entry.UniqueId != UniqueStageEntryId);
        return linkableEntries.Any() ? new List<IStageEntry>(linkableEntries) : new List<IStageEntry>();
      }
    }
  }
}
