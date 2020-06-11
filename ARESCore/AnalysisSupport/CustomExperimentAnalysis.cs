using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using ARESCore.Database.Filtering.Impl;
using ARESCore.Database.Tables;
using ARESCore.DataHub;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using Ninject;
using ReactiveUI;

namespace ARESCore.AnalysisSupport
{
  public class CustomExperimentAnalysis : BasicReactiveObjectDisposable, IAresAnalyzer
  {
    private readonly IDataHub _hub = ServiceLocator.Current.GetInstance<IDataHub>();
    private readonly DataEntity _DataEntity;
    private List<Point> _calibPoints = new List<Point>();
    private readonly List<KeyValuePair<Color, Point>> _aOIBandsPoints = new List<KeyValuePair<Color, Point>>();
    private bool _isSelected;
    private readonly IDisposable _runningSubscription = null;
    public List<string> Headers { get; set; } = new List<string>();
    public UserControl AnalysisDbFilter { get; set; } = new CustomAnalysisDbFilter();

    public List<Point> CalibDataPoints
    {
      get => _calibPoints;
      set => this.RaiseAndSetIfChanged(ref _calibPoints, value);
    }
    public void CalculateFit(List<DataEntity> spectralDocuments, List<MachineStateEntity> machineDocuments)
    {
      // do nothing
    }

    public string GetPostProcessOverview(IAresAnalyzer referenceProcess)
    {
      // do nothing
      return "";
    }

    public void TrySet(string currentDesc, string lineToken)
    {
      // do nothing
    }

    public string Tokenize(string header)
    {
      return "";
    }

    public IEnumerable<string> GetPrimaryAnalysisValues()
    {
      List<string> toReturn = new List<string>();
      return toReturn;
    }

    public IEnumerable<string> GetSecondaryAnalysisValues()
    {
      List<string> toReturn = new List<string>();
      return toReturn;
    }

    public List<List<string>> GetFilteredDataInRows(Type inputType)
    {
      var filres = AresKernel._kernel.Get<DbFilterManager>().LastFilterResult as IEnumerable<ExperimentEntity>;
      var headers = new List<string>() { "Project", "Date", };
      var toReturn = new List<List<string>>
      {
        headers
      };
      foreach (var exp in filres)
      {
        var entry = new List<string>
        {
          exp.Project,
          exp.TimeStamp.ToShortDateString()
        };
        toReturn.Add(entry);
      }
      return toReturn;
    }

    public Type DbTypeSupported { get; } = typeof(string);

    public string AnalyzerName { get; set; } = "None";
    public bool IsSelected
    {
      get => _isSelected;
      set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
  }
}
