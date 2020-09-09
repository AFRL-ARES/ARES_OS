using AresAnalysisPlugin.Analyzers;
using AresAnalysisPlugin.Management;
using System;
using System.Windows.Controls;

namespace AresAnalysisPlugin.UI.Views
{
  /// <summary>
  /// Interaction logic for PointAnalyzerSetter.xaml
  /// </summary>
  public partial class PointAnalyzerSetter : UserControl, IAnalyzerSetter
  {
    public PointAnalyzerSetter()
    {
      InitializeComponent();
    }

    public Type AnalyzerTypeSupported { get; set; } = typeof(PointAnalyzer);
  }
}
