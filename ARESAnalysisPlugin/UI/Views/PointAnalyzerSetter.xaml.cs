using AresFCAnalysisPlugin.Analyzers;
using AresFCAnalysisPlugin.Management;
using System;
using System.Windows.Controls;

namespace AresFCAnalysisPlugin.UI.Views
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
