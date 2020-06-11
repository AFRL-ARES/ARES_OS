using System;
using System.Linq;
using AresAdditiveDevicesPlugin.Processing.Staging;
using ARESCore.DisposePatternHelpers;
using ARESCore.Extensions;
using CommonServiceLocator;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base.Vms
{
  public class ProcessDataInputRowViewModel : BasicReactiveObjectDisposable
  {
    //    private IPipelineService _pipelineService;
    private IProcessData _input;
    private IProcessData _inputEdit;
    private IStageEntry _linkedEntry;
    private IStageEntry _linkedEntryEdit;
    private IProcessData _linkedInput;
    private IProcessData _linkedInputEdit;
    private bool _isLinked;
    private bool _isLinkedEdit;
    private bool _isValid;

    public ProcessDataInputRowViewModel(
      IProcessData input,
      ComponentEditorViewModel componentEditor)
    {
      Input = input;
      IsLinked = false;

      Disposables.Add(componentEditor.Subscribe(compEditor => compEditor.EditsApplied, compEditor => OnCanceledOrApplied(compEditor.EditsApplied)));

    }

    private void OnCanceledOrApplied(bool applied)
    {
      if (applied)
      {
        Apply();
      }
      else
      {
        ResetEdit();
      }
    }

    public void Apply()
    {
      IsLinked = IsLinkedEdit;
      LinkedEntry = LinkedEntryEdit;
      LinkedInput = LinkedInputEdit;
      Input = InputEdit;
      if (IsLinked)
      {
        Input = LinkedInput;
      }
    }

    public void ResetEdit()
    {
      InputEdit = Input;
      IsLinkedEdit = IsLinked;
      LinkedEntryEdit = LinkedEntry;
      LinkedInputEdit = LinkedInput;
    }

    private bool CheckValidity()
    {
      return CheckValidLink();
    }

    private bool CheckValidLink()
    {
      if (!IsLinkedEdit)
      {
        return true;
      }

      if (LinkedEntryEdit == null || LinkedInputEdit == null)
      {
        return false;
      }

      // We are linked to an entry's input
      var linkedInputIndex = LinkedEntryEdit.Inputs.IndexOf(LinkedInputEdit);


      // The linked entry's input is also linked
      var linkedEntryDerivative = LinkedEntryEdit;
      while (linkedEntryDerivative.InputMappings.Any(mapping => mapping.InputIndex == linkedInputIndex))
      {
        var linkedEntryMapping = linkedEntryDerivative.InputMappings.FirstOrDefault(mapping => mapping.InputIndex == linkedInputIndex);
        linkedInputIndex = linkedEntryMapping.LinkEntryInputIndex;
        if (linkedEntryMapping.LinkedEntryId == Guid.Empty)
        {
          return false;
        }
        var linkedEntryDerivativeLookup = ServiceLocator.Current.GetInstance<IPipelineService>().AllStageEntries.Lookup(linkedEntryMapping.LinkedEntryId);
        if (linkedEntryDerivativeLookup.HasValue)
        {
          linkedEntryDerivative = linkedEntryDerivativeLookup.Value;
        }
        else
        {
          return false;
        }
      }

      // Found a static reference through a path of valid links
      return linkedInputIndex >= 0 && LinkedEntryEdit.Inputs.Contains(LinkedInputEdit) && InputEdit.Type.IsAssignableFrom(LinkedInputEdit.Type);
      ;
    }

    public bool IsValid
    {
      get => _isValid;
      set => this.RaiseAndSetIfChanged(ref _isValid, value);
    }

    public IProcessData Input
    {
      get => _input;
      set => this.RaiseAndSetIfChanged(ref _input, value);
    }

    public bool IsLinked
    {
      get => _isLinked;
      set
      {
        this.RaiseAndSetIfChanged(ref _isLinked, value);
        IsValid = CheckValidity();
      }
    }

    public IStageEntry LinkedEntry
    {
      get => _linkedEntry;
      set
      {
        this.RaiseAndSetIfChanged(ref _linkedEntry, value);
        IsValid = CheckValidity();
      }
    }

    public IProcessData LinkedInput
    {
      get => _linkedInput;
      set
      {
        this.RaiseAndSetIfChanged(ref _linkedInput, value);
        IsValid = CheckValidity();
      }
    }

    public IProcessData InputEdit
    {
      get => _inputEdit;
      set
      {
        this.RaiseAndSetIfChanged(ref _inputEdit, value);
        IsValid = CheckValidity();
      }
    }

    public bool IsLinkedEdit
    {
      get => _isLinkedEdit;
      set
      {
        this.RaiseAndSetIfChanged(ref _isLinkedEdit, value);
        IsValid = CheckValidity();
      }
    }

    public IStageEntry LinkedEntryEdit
    {
      get => _linkedEntryEdit;
      set
      {
        this.RaiseAndSetIfChanged(ref _linkedEntryEdit, value);
        IsValid = CheckValidity();
      }
    }

    public IProcessData LinkedInputEdit
    {
      get => _linkedInputEdit;
      set
      {
        this.RaiseAndSetIfChanged(ref _linkedInputEdit, value);
        IsValid = CheckValidity();
      }
    }
  }
}
