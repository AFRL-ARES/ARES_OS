using System.Windows;
using System.Windows.Controls;
using AresSampleAnalysisPlugin.Analyzers.Sample.Ui.Vms;

namespace AresSampleAnalysisPlugin.Analyzers.Sample.Ui.Views
{
  /// <summary>
  /// Interaction logic for SampleAnalyzerView.xaml
  /// </summary>
  public partial class SampleAnalyzerView
  {
    public SampleAnalyzerView()
    {
      InitializeComponent();
    }

    private void SampleAnalyzerView_OnLoaded(object sender, RoutedEventArgs e)
    {
      var vm = DataContext as SampleAnalyzerViewModel;
      if (vm.SamplePlot == null)
      {
        vm.SamplePlot = SamplePlot;
        vm.Setup();
      }
    }
  }
}
