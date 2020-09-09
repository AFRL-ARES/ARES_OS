using AresAnalysisPlugin.Analyzers;
using AresAnalysisPlugin.Management;
using System;
using System.Windows.Controls;

namespace AresAnalysisPlugin.UI.Views
{
  /// <summary>
  /// Interaction logic for MaxInRangeSetter.xaml
  /// </summary>
  public partial class MaxInRangeSetter : UserControl, IAnalyzerSetter
  {
    public MaxInRangeSetter()
    {
      InitializeComponent();
    }

    public Type AnalyzerTypeSupported { get; set; } = typeof(MaxInRangeAnalyzer);
  }
}
