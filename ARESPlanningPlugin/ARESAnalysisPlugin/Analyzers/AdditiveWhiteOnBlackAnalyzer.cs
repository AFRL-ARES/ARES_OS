using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using AresAdditiveDevicesPlugin.Processing.Components;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using AresAdditiveDevicesPlugin.PythonStageController;
using ARESCore.AnalysisSupport;
using ARESCore.DataHub;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Results;
using ARESCore.Extensions;
using CommonServiceLocator;
using ReactiveUI;

namespace AresAdditiveAnalysisPlugin.Analyzers
{
  public class AdditiveWhiteOnBlackAnalyzer : BasicReactiveObjectDisposable, IAresAnalyzer
  {
    public static string SourceName = "WhiteOnBlackAnalysis";
    private bool _isSelected;
    private IDisposable _runningSubscription = null;
    public List<string> Headers { get; set; }
    public UserControl AnalysisDbFilter { get; set; }


    private async Task ProcessNewData(IDataHubEntry data)
    {
      if (data == null)
      {
        return;
      }
      if (data.DataType == typeof(Bitmap))
      {
        var stageController = ServiceLocator.Current.GetInstance<IStageController>();
        var campaignSummary = ServiceLocator.Current.GetInstance<ICampaignExecutionSummary>();
        var experimentSummary = campaignSummary.ExperimentExecutionSummaries.Last();
        await stageController.WritePyDict("data.Expt#", experimentSummary.ExperimentNumber);

        var componentService = ServiceLocator.Current.GetInstance<IComponentService>();
        var whiteOnBlackAnalysisComponent = (BasicUserDefinedComponent)componentService.AllComponents.Items.First(component =>
          component is BasicUserDefinedComponent && component.ComponentName == AdditiveWhiteOnBlackAnalyzer.SourceName);
        var image = (Bitmap)data.Data;
        whiteOnBlackAnalysisComponent.AnalysisImage = new Bitmap(image);
        await whiteOnBlackAnalysisComponent.StartComponent(null);
        var result = whiteOnBlackAnalysisComponent.Result;
        experimentSummary.Result = result;
      }

    }

    public string GetPostProcessOverview(IAresAnalyzer referenceProcess)
    {
      throw new NotImplementedException();
    }

    public void TrySet(string currentDesc, string lineToken)
    {
      throw new NotImplementedException();
    }

    public string Tokenize(string header)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> GetPrimaryAnalysisValues()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> GetSecondaryAnalysisValues()
    {
      throw new NotImplementedException();
    }

    public List<List<string>> GetFilteredDataInRows(Type inputType)
    {
      throw new NotImplementedException();
    }

    public Type DbTypeSupported { get; }
    public string AnalyzerName { get; set; } = "White On Black";
    public bool IsSelected
    {
      get => _isSelected;
      set
      {
        this.RaiseAndSetIfChanged(ref _isSelected, value);
        if (value)
        {
          _runningSubscription = ServiceLocator.Current.GetInstance<IDataHub>()
            .Subscribe(dataHub => dataHub.Data, dataHub => ProcessNewData(dataHub.Data).Wait());
        }
        else
        {
          _runningSubscription?.Dispose();
        }
      }
    }
  }
}
