using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.Processing.Components;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.Processing.Impl
{
  public class ProcessPipelineSaver : BasicReactiveObjectDisposable
  {
    private readonly IPipelineService _pipelineService;

    public ProcessPipelineSaver(
        IPipelineService pipelineService,
        IObservable<SavePipeLine> savePipeLineEventObservable)
    {
      _pipelineService = pipelineService;
      var savePipeLineEventWatcher = savePipeLineEventObservable
          .ObserveOn(NewThreadScheduler.Default)
          .Subscribe(async x => await SavePipeline());

      Disposables.Add(savePipeLineEventWatcher);
    }

    private Task SavePipeline()
    {
      var orderedEntries = _pipelineService.AllStageEntries.Items
          .OrderBy(stageEntry => stageEntry.StageId)
          .ThenBy(stageEntry => stageEntry.Process.ComponentName);

      var pipeline = new ProcessPipeline();
      pipeline.AddRange(orderedEntries);
      var currentPath = Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\..\Analyzers");


      var saveFileDialog = new SaveFileDialog
      {
        Title = "Save New Proces",
        Filter = "json files (*.json)|*.json",
        DefaultExt = "json",
        InitialDirectory = currentPath

      };
      saveFileDialog.ShowDialog();

      if (saveFileDialog.FileName != "")
      {
        var userDefinedComponent = new BasicUserDefinedComponent(pipeline, Path.GetFileNameWithoutExtension(saveFileDialog.FileName));
        var jComponent = userDefinedComponent.SerializeObject();
        File.WriteAllText(saveFileDialog.FileName, jComponent);
        var componentService = ServiceLocator.Current.GetInstance<IComponentService>();
        componentService.Add(userDefinedComponent);
      }

      return Task.CompletedTask;
    }
  }
}
