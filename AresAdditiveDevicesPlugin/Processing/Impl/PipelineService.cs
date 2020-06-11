using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.Processing.Staging;
using ARESCore.DisposePatternHelpers;
using ARESCore.Extensions;
using DynamicData;
using MoreLinq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AresAdditiveDevicesPlugin.Processing.Impl
{
  public class PipelineService : BasicReactiveObjectDisposable, IPipelineService
  {
    private int _numStages;

    private SourceCache<IStageEntry, Guid> StageEntries { get; } = new SourceCache<IStageEntry, Guid>(stageEntry => stageEntry.UniqueId);

    public PipelineService(EventHub eventHub)
    {
      AllStageEntries = StageEntries.AsObservableCache();

      var stageEntryRemovalListener = eventHub.GetEvent<RemoveStageEntry>().Subscribe(removeEvent => RemoveEntry(removeEvent.StageEntry));

      Disposables.Add(stageEntryRemovalListener);

    }

    public IObservableCache<IStageEntry, Guid> AllStageEntries { get; set; }


    public void RemoveEntry(IStageEntry stageEntry)
    {
      StageEntries.Remove(stageEntry);

      var affectedEntryToMappingLookup = new Dictionary<Guid, StageEntryInputMappings>();

      // Entries directly linked to this removed entry
      var directlyAffectedEntries = AllStageEntries.Items.Where(entry => entry.InputMappings.Select(mapping => mapping.LinkedEntryId).Contains(stageEntry.UniqueId));
      directlyAffectedEntries.ForEach(entry =>
      {
        var entryId = entry.UniqueId;
        if (!affectedEntryToMappingLookup.TryGetValue(entryId, out var existingMappings))
        {
          existingMappings = new StageEntryInputMappings();
        }
        var invalidatedMappings = entry.InputMappings.Where(mapping => stageEntry.UniqueId == mapping.LinkedEntryId);
        invalidatedMappings.ForEach(mapping => existingMappings.Add(mapping));
        affectedEntryToMappingLookup.Add(entryId, existingMappings);
      });

      // Cascaded entries linked to removed entry
      var lowestAffectedStage = stageEntry.StageId;
      if (!AllStageEntries.Items.Any())
      {
        return;
      }
      var maxStage = AllStageEntries.Items.Max(entry => entry.StageId);
      for (var stage = lowestAffectedStage; stage < maxStage; stage++)
      {
        var stageAffectedEntries = AllStageEntries.Items.Where(entry => entry.StageId == stage && entry.InputMappings.Any(mapping => affectedEntryToMappingLookup.ContainsKey(mapping.LinkedEntryId)));
        stageAffectedEntries.ForEach(entry =>
        {
          var entryId = entry.UniqueId;
          if (!affectedEntryToMappingLookup.TryGetValue(entryId, out var existingMappings))
          {
            existingMappings = new StageEntryInputMappings();
          }
          var invalidatedMappings = entry.InputMappings.Where(mapping => affectedEntryToMappingLookup.ContainsKey(mapping.LinkedEntryId));
          invalidatedMappings.ForEach(mapping => existingMappings.Add(mapping));
          affectedEntryToMappingLookup.Add(entryId, existingMappings);
        });
      }

      affectedEntryToMappingLookup.ForEach(affectedMapping =>
      {
        var affectedEntry = AllStageEntries.Lookup(affectedMapping.Key).Value;
        affectedEntry.InputMappings.RemoveWhere(mapping => affectedMapping.Value.Select(brokenMapping => brokenMapping.InputIndex).Contains(mapping.InputIndex));
      });

      NumberOfStages = !AllStageEntries.Items.Any() ? 1 : AllStageEntries.Items.Max(entry => entry.StageId);
    }

    public void GrabEntry(IStageEntry stageEntry)
    {
      StageEntries.Remove(stageEntry);
    }

    public void Add(IStageEntry stageEntry)
    {
      if (!StageEntries.Lookup(stageEntry.UniqueId).HasValue)
      {
        stageEntry.SubscribeAndInvoke(entry => entry.StageId, entry => NumberOfStages = (AllStageEntries.Items == null || !AllStageEntries.Items.Any()) ? 1 : Math.Max(entry.StageId, AllStageEntries.Items.Max(collectedEntry => collectedEntry.StageId)));
      }
      StageEntries.AddOrUpdate(stageEntry);
    }

    public void Clear()
    {
      StageEntries.Clear();
    }

    public void RunPipe()
    {
    }

    public IEnumerable<Guid> StageEntryIdsDependentTo(Guid targetId)
    {
      var dependentEntriesLookup = new HashSet<Guid>();
      if (targetId == Guid.Empty)
      {
        return dependentEntriesLookup;
      }
      // The entries directly dependent to the target entry
      var dependentEntries = AllStageEntries.Items.Where(entry => entry.InputMappings.Select(mapping => mapping.LinkedEntryId).Contains(targetId));

      int previousCount = -1;
      int currentCount = 0;
      while (dependentEntries.Any() && currentCount > previousCount)
      {
        previousCount = currentCount;
        dependentEntriesLookup.AddRange(dependentEntries.Select(entry => entry.UniqueId));
        currentCount = dependentEntriesLookup.Count;
        // Cascaded dependent entries
        dependentEntries = AllStageEntries.Items.Where(entry => entry.InputMappings.Select(mapping => mapping.LinkedEntryId).Any(linkedId => dependentEntriesLookup.Contains(linkedId))).ToArray();
      }

      return dependentEntriesLookup;
    }

    public int NumberOfStages
    {
      get => _numStages;
      set => this.RaiseAndSetIfChanged(ref _numStages, value);
    }
  }
}
