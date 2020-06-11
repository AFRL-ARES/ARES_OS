using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Experiment;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Json;
using ARESCore.DisposePatternHelpers;
using ARESCore.Extensions;
using CommonServiceLocator;
using DynamicData;
using DynamicData.Binding;
using MoreLinq;
using Newtonsoft.Json;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Staging.Impl
{
  [Serializable]
  public class StageEntry : BasicReactiveObjectDisposable, IStageEntry
  {
    private int _stageId = 1;
    private Guid _uniqueId;
    private IComponent _process;
    private IObservableCollection<IProcessData> _inputs;
    private StageEntryInputMappings _inputMappings = new StageEntryInputMappings();
    private Dictionary<Guid, IDisposable> _linkedEntrySubscriptions = new Dictionary<Guid, IDisposable>();

    public StageEntry() { }

    public StageEntry(IComponent process)
    {
      Process = process;
      Inputs = new ObservableCollectionExtended<IProcessData>();
      Inputs.AddRange(process.DefaultInputs);

      Disposables.Add(InputMappings.SubscribeAndInvoke(SubscribeToStageIdChanges, OnInputMappingRemoved));
    }

    [JsonConstructor]
    public StageEntry(IComponent process, IObservableCollection<IProcessData> inputs, int stageId, Guid uniqueId, StageEntryInputMappings pendingStageEntries)
    {
      Process = process;
      Inputs = inputs;
      UniqueId = uniqueId;
      StageId = stageId;
      InputMappings = pendingStageEntries;

      Disposables.Add(InputMappings.SubscribeAndInvoke(SubscribeToStageIdChanges, OnInputMappingRemoved));

    }

    public StageEntry(IStageEntry sourceEntry)
    {
      Process = sourceEntry.Process;
      Inputs = new ObservableCollectionExtended<IProcessData>();
      Inputs.AddRange(sourceEntry.Inputs);
      UniqueId = sourceEntry.UniqueId;

      InputMappings = sourceEntry.InputMappings;
      Disposables.Add(InputMappings.SubscribeAndInvoke(SubscribeToStageIdChanges, OnInputMappingRemoved));
    }

    private void SubscribeToStageIdChanges(StageEntryInputMapping mapping)
    {
      if (_linkedEntrySubscriptions.ContainsKey(mapping.LinkedEntryId))
      {
        // Already subscribed to entry
        return;
      }
      var pendingStageEntryLookup = ServiceLocator.Current.GetInstance<IPipelineService>().AllStageEntries.Lookup(mapping.LinkedEntryId);
      if (!pendingStageEntryLookup.HasValue)
      {
        return;
      }
      // Any time a linked entry's Stage changes, update our own
      var linkedSubscription = pendingStageEntryLookup.Value.SubscribeAndInvoke(pendingEntry => pendingEntry.StageId, OnLinkedEntryStageIdChanged);
      _linkedEntrySubscriptions.Add(mapping.LinkedEntryId, linkedSubscription);
      Disposables.Add(_linkedEntrySubscriptions[mapping.LinkedEntryId]);
    }

    private void OnInputMappingRemoved(StageEntryInputMapping mapping)
    {
      if (!_linkedEntrySubscriptions.ContainsKey(mapping.LinkedEntryId))
      {
        return;
      }
      _linkedEntrySubscriptions[mapping.LinkedEntryId].Dispose();
      _linkedEntrySubscriptions.Remove(mapping.LinkedEntryId);
      StageId = AssignStageId();
    }

    private void OnLinkedEntryStageIdChanged(IStageEntry linkedEntry)
    {
      StageId = Math.Max(linkedEntry.StageId + 1, AssignStageId());
    }

    public int AssignStageId()
    {
      if (InputMappings.Count == 0)
      {
        return 1;
      }

      var linkedEntries = ServiceLocator.Current.GetInstance<IPipelineService>()
        .AllStageEntries
        .Items
        .Where(entry =>
         InputMappings
         .Select(mapping => mapping.LinkedEntryId)
         .Contains(entry.UniqueId));
      var stageEntries = linkedEntries as IStageEntry[] ?? linkedEntries.ToArray();
      if (!stageEntries.Any())
      {
        return 1;
      }
      return stageEntries.Max(entry => entry.StageId) + 1;
    }

    //    private Task<List<IProcessData>> LinkEntries()
    private Task LinkEntries()
    {
      var linked = new List<IProcessData>();
      Inputs.ForEach((input, index) =>
      {
        var indexMapping = InputMappings.FirstOrDefault(mapping => mapping.InputIndex == index);
        if (indexMapping == null)
        {
          linked.Add(input);
          return;
        }
        var linkedEntry = ServiceLocator.Current.GetInstance<IExperimentCampaign>().AnalysisComponent.Pipeline.FirstOrDefault(entry => indexMapping.LinkedEntryId == entry.UniqueId);
        input.Data = linkedEntry.Inputs[indexMapping.LinkEntryInputIndex].Data;
        linked.Add(input);
      });
      return Task.CompletedTask;
    }

    public async Task ExecuteProcess()
    {
      await LinkEntries();
      await Process.StartComponent(Inputs);
    }

    [JsonProperty("StageId")]
    public int StageId
    {
      get => _stageId;
      set => this.RaiseAndSetIfChanged(ref _stageId, value);
    }

    [JsonProperty("UniqueId")]
    public Guid UniqueId
    {
      get => _uniqueId;
      set => this.RaiseAndSetIfChanged(ref _uniqueId, value);
    }

    [JsonProperty("Process")]
    public IComponent Process
    {
      get => _process;
      set => this.RaiseAndSetIfChanged(ref _process, value);
    }


    [JsonProperty("Inputs")]
    [JsonConverter(typeof(CustomJsonConverter))]
    public IObservableCollection<IProcessData> Inputs
    {
      get => _inputs;
      set => this.RaiseAndSetIfChanged(ref _inputs, value);
    }

    [JsonProperty("InputMappings")]
    public StageEntryInputMappings InputMappings
    {
      get => _inputMappings;
      set => this.RaiseAndSetIfChanged(ref _inputMappings, value);
    }

    public override string ToString()
    {
      return Process.ComponentName + " " + StageId;
    }
  }
}
