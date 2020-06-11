using System.Windows;
using System.Windows.Controls;

namespace AresFCAnalysisPlugin
{
  /// <summary>
  /// Interaction logic for AnalysisControl.xaml
  /// </summary>
  public partial class AnalysisControl : UserControl
  {
    public AnalysisControl()
    {
      InitializeComponent();
    }

    private void CreatePopup(object sender, RoutedEventArgs e)
    {
      _selectorPopup.IsOpen = true;
    }

    private void SelectorClicked(object sender, RoutedEventArgs e)
    {
      _selectorPopup.IsOpen = false;
    }
  }
}
