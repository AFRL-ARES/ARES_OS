using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Processing.Components.Base;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Json;
using AresAdditiveDevicesPlugin.PythonInterop;
using ARESCore.Experiment.Results;
using CommonServiceLocator;
using DynamicData;
using MoreLinq;
using Newtonsoft.Json;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Components.UserDefined
{
  [JsonConverter(typeof(BasicUserDefinedComponentJsonConverter))]
  public class BasicUserDefinedComponent : BasicComponent
  {
    private ProcessPipeline _pipline;
    private double _result;

    public BasicUserDefinedComponent(ProcessPipeline pipeline, string name)
    {
      Pipeline = pipeline;
      DefaultInputs.AddRange(GetAssignableInputs());
      ComponentName = name;
    }


    private IEnumerable<IProcessData> GetAssignableInputs()
    {
      var manualInputs = Pipeline
        .Select(entry => entry.Inputs.Select(input => input)
         .Where(input => input.RequiresSetup))
        .Flatten()
        .Cast<IProcessData>();
      return manualInputs;
    }


    public override async Task StartComponent(IList<IProcessData> inputs)
    {
      await Pipeline.Execute();
      var resultHolder = Pipeline
        .Select(entry => entry.Inputs.Select(input => input)
         .Where(input => input.IsResult))
        .Flatten()
        .Cast<IProcessData>()
        .FirstOrDefault();

      Result = (double)resultHolder.Data;

    }

    [JsonIgnore]
    public static string HumanReadableTypeString = "User Defined";

    private Bitmap _analysisImage;


    [JsonProperty("Pipeline")]
    [JsonConverter(typeof(ProcessPipelineJsonConverter))]
    public ProcessPipeline Pipeline
    {
      get => _pipline;
      set => this.RaiseAndSetIfChanged(ref _pipline, value);
    }

    [JsonIgnore]
    public double Result
    {
      get => _result;
      set => this.RaiseAndSetIfChanged(ref _result, value);
    }

    [JsonIgnore]
    public Bitmap AnalysisImage
    {
      get => _analysisImage;
      set
      {
        if (_analysisImage != value)
        {
          _analysisImage?.Dispose();

          var imgSourceEntry = Pipeline.FirstOrDefault(entry => entry.Inputs.FirstOrDefault(input => input.IsAnalysisImage).IsAnalysisImage);
          var imgInput = imgSourceEntry.Inputs.FirstOrDefault(input => input.IsAnalysisImage);
          if (imgSourceEntry.Process is IPythonProcess)
          {
            //            var imageFileDir = new DirectoryInfo(@"..\..\..\Images\Experiment\");
            //            var imageFile = (from file in imageFileDir.GetFiles()
            //                             orderby file.LastWriteTime descending
            //                             select file).FirstOrDefault();
            //            if (imageFile != null)
            //            {
            //              imgInput.Data = imageFile.FullName;
            //            }
            //            else
            //            {
            //            }


            var campaignExecution = ServiceLocator.Current.GetInstance<ICampaignExecutionSummary>();
            var currentExperimentNumber = campaignExecution.ExperimentExecutionSummaries.Last().ExperimentNumber;
            var campaignsLocation =
              new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.GetDirectories().First(dir =>
                dir.Name.Equals("Campaigns", StringComparison.CurrentCultureIgnoreCase));
            var currentCampaignLocation = campaignsLocation.GetDirectories().OrderBy(dir => dir.CreationTime).Last();
            var currentExperimentLocation = currentCampaignLocation.GetDirectories()
              .First(dir => dir.Name.Equals($"Experiment_{currentExperimentNumber:000}"));
            var imgFile = currentExperimentLocation.GetFiles().OrderBy(file => file.CreationTime).Last();
            imgInput.Data = imgFile.FullName;
          }

          _analysisImage = value;
          this.RaisePropertyChanged();
        }

      }

    }
  }
}
