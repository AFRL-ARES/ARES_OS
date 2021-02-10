using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using ARESCore.Experiment.Results;
using CommonServiceLocator;
using DynamicData;
using DynamicData.Binding;
using OxyPlot;
using OxyPlot.Wpf;
using ReactiveUI;

namespace AresSampleAnalysisPlugin.Analyzers.Sample.Ui.Vms
{
  public class SampleAnalyzerViewModel : ReactiveObject
  {
    private LineSeries _sampleLine = new LineSeries();
    private Plot _samplePlot;
    private List<LineSeries> _sampleContent = new List<LineSeries>();


    public void Setup()
    {
      var results = new ObservableCollectionExtended<double>();
      var campaignExecutionSummary = ServiceLocator.Current.GetInstance<ICampaignExecutionSummary>();
      campaignExecutionSummary
        .ExperimentExecutionSummaries
        .ToObservableChangeSet()
        .Transform
          (
           expExecSum =>
             expExecSum.WhenPropertyChanged(expSum => expSum.Result, false)
                       .Take(1)
                       .Subscribe(resultChange => results.Add(resultChange.Sender.Result))
          )
        .Subscribe();
      results.ToObservableChangeSet()
             .Transform(experimentResult => new DataPoint(results.IndexOf(experimentResult), experimentResult))
             .Bind( out var sampleData)
             .Subscribe();
      SampleData = sampleData;

      InitializeModelAndLines();
    }

//    private void UpdateResults()
//    {
//      var copy = new List<AoiHubData>(_aois);
//      var existingAoi = copy.FirstOrDefault(a => a.Name.Equals(aoi.Name));
//      if (existingAoi != null)
//      {
//        _aois.Remove(existingAoi);
//        if (_drawnAois.ContainsKey(existingAoi))
//        {
//          var data = _drawnAois[existingAoi];
//          _drawnAois.Remove(existingAoi);
//          Application.Current.Dispatcher.Invoke(() => { Plot.Series.Remove(data); });
//        }
//      }
//
//      if (!aoi.Draw)
//        return;
//
//      List<LineSeries> lines = new List<LineSeries>();
//      var leftPlot = CreateLineMarker(aoi.DrawColor, aoi.LeftLimit);
//      lines.Add(leftPlot);
//      var rightPlot = CreateLineMarker(aoi.DrawColor, aoi.RightLimit);
//      lines.Add(rightPlot);
//      if (aoi.Background != null)
//      {
//        var bg = Application.Current.Dispatcher.Invoke(() =>
//                                                       {
//                                                         var line = new LineSeries();
//                                                         var points = new List<DataPoint>();
//                                                         foreach (var point in aoi.Background)
//                                                         {
//                                                           var pt = new DataPoint(point.X, point.Y);
//                                                           points.Add(pt);
//                                                         }
//                                                         line.Color = aoi.DrawColor;
//                                                         line.LineStyle = LineStyle.DashDashDot;
//                                                         line.StrokeThickness = 1.5f;
//                                                         line.ItemsSource = points;
//                                                         return line;
//                                                       });
//      }
//
//      _drawnAois[aoi] = lines;
//      _aois.Add(aoi);
//      Application.Current.Dispatcher.Invoke(() =>
//                                            {
//                                              Plot.Series.Add(lines);
//                                              if (Plot.Axes[0].Maximum > 20000)
//                                                Plot.Axes[0].Maximum = RamanSpectrum.Max(v => v.Y) * 1.05;
//                                              Plot.InvalidatePlot();
//                                            });
//    }

    private void InitializeModelAndLines()
    {
      SamplePlot.Series.Add(_sampleLine);
    }


    public Plot SamplePlot
    {
      get => _samplePlot;
      set => this.RaiseAndSetIfChanged(ref _samplePlot, value);
    }

    public ReadOnlyObservableCollection<DataPoint> SampleData { get; set; }

    public void ResetSamplePlot()
    {
      foreach (var line in _sampleContent)
      {
        SamplePlot.Series.Remove(line);
      }
      _sampleContent.Clear();
      SamplePlot.InvalidatePlot();
    }

    public void AddToSample(LineSeries dataToAdd, bool scaleLineHeightToPlot, bool scaleMinMaxToData)
    {
      _sampleContent.Add(dataToAdd);
      SamplePlot.Series.Add(dataToAdd);
      if (scaleLineHeightToPlot)
      {
        var maxy = SampleData.Max(v => v.Y);
        var miny = SampleData.Min(v => v.Y);
        SamplePlot.Axes[0].Maximum = maxy * 1.1;
        SamplePlot.Axes[0].Minimum = miny * 0.9;
      }

      if (scaleMinMaxToData)
      {
        var totMinX = _sampleContent.Min(l => ((List<DataPoint>)l.ItemsSource).Min(m => m.X));
        var totMaxX = _sampleContent.Max(l => ((List<DataPoint>)l.ItemsSource).Max(m => m.X));
        SamplePlot.Axes[1].Minimum = totMinX * 0.9;
        SamplePlot.Axes[1].Maximum = totMaxX * 1.1;
      }

      SamplePlot.InvalidatePlot();
    }
  }
}
