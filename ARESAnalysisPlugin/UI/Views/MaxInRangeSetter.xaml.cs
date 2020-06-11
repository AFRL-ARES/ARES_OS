using AresFCAnalysisPlugin.Analyzers;
using AresFCAnalysisPlugin.Management;
using System;
using System.Windows.Controls;

namespace AresFCAnalysisPlugin.UI.Views
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
