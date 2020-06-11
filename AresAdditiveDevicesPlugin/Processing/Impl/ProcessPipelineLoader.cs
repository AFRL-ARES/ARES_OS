using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using AresAdditiveDevicesPlugin.Processing.Staging;
using ARESCore.DisposePatternHelpers;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AresAdditiveDevicesPlugin.Processing.Impl
{
    public class ProcessPipelineLoader : BasicDisposable
    {
        public ProcessPipelineLoader(
            IPipelineService pipelineService,
            IObservable<LoadPipeLine> loadEventWatcherObservable)
        {
            var loadEventWatcher =
                loadEventWatcherObservable
                    .ObserveOnDispatcher()
                    .Select(line =>
                    {
                        pipelineService.Clear();
                        var openFileDialog = new OpenFileDialog { Title = "Select a Process", Filter = "json files (*.json)|*.json" };

                        List<IStageEntry> entries = new List<IStageEntry>();
                        if (openFileDialog.ShowDialog() == true)
                        {
                            var serialized = File.ReadAllText(openFileDialog.FileName);
                            try
                            {
                                var deserializedEntry = JsonConvert.DeserializeObject<BasicUserDefinedComponent>(serialized);
                                entries.AddRange(deserializedEntry.Pipeline);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        return entries;
                    })
                    .Where(list => list.Any())
                    .ObserveOn(DispatcherScheduler.Current)
                    .Subscribe(list => list.ForEach(stageEntry => pipelineService.Add(stageEntry)));

            Disposables.Add(loadEventWatcher);
        }
    }
}
