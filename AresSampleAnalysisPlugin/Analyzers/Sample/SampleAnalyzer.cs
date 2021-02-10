using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using ARESCore.AnalysisSupport;
using ARESCore.DataHub;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Results;
using DynamicData.Binding;
using ReactiveUI;

namespace AresSampleAnalysisPlugin.Analyzers.Sample
{
  public class SampleAnalyzer : ReactiveSubscriber, IAresAnalyzer
  {
    public static string SourceName = "SampleAnalysis";


    private List<string> _headers;
    private UserControl _analysisDbFilter;
    private Type _dbTypeSupported;
    private string _analyzerName;
    private bool _isSelected;
    private ICampaignExecutionSummary _campaignExecutionSummary;
    private IDataHub _dataHub;
    private IDisposable _runningSubscription;

    public SampleAnalyzer(IDataHub dataHub, ICampaignExecutionSummary campaignExecutionSummary )
    {
    _campaignExecutionSummary = campaignExecutionSummary;
    _dataHub = dataHub;
    Disposables.Add(this.WhenPropertyChanged(analyzer => analyzer.IsSelected).Subscribe(selectionChange => HandleAnalyzerSelection(selectionChange.Value)));
  }

  private void HandleAnalyzerSelection(bool selected)
  {
  if (!selected)
  {
    _runningSubscription?.Dispose();
    return;
  }
  _runningSubscription =
  _dataHub
    .WhenPropertyChanged(hub => hub.Data, false)
  .Subscribe(async dataChange => await ProcessNewData(dataChange.Value));
  }

  private async Task ProcessNewData(IDataHubEntry dataEntry)
  {
    var experimentSummary = _campaignExecutionSummary.ExperimentExecutionSummaries.LastOrDefault();
    if (experimentSummary == null)
    {
      return;
    }

    var rand = new Random().NextDouble();
    experimentSummary.ResultBase = "Sample";
    experimentSummary.Result = rand;
  }

  public List<string> Headers
    {
      get => _headers;
      set => _headers = value;
    }

    public UserControl AnalysisDbFilter
    {
      get => _analysisDbFilter;
      set => _analysisDbFilter = value;
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

    public Type DbTypeSupported
    {
      get => _dbTypeSupported;
    }

    public string AnalyzerName { get; set; } = "Sample Analyzer";

    public bool IsSelected
    {
      get => _isSelected;
      set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
  }
}
