using AresAdditiveDevicesPlugin.PythonStageController.UI.Vms;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace AresAdditiveDevicesPlugin.PythonStageController.UI.Views
{
  /// <summary>
  /// Interaction logic for ToolpathConfiguratorView.xaml
  /// </summary>
  public partial class ToolpathConfiguratorView : UserControl
  {
    public ToolpathConfiguratorView()
    {
      InitializeComponent();
    }
    private void SelectFile(object sender, RoutedEventArgs e)
    {
      var selector = new OpenFileDialog();
      if (selector.ShowDialog() == true)
      {
        (DataContext as ToolpathConfiguratorViewModel).ToolpathFilepath = selector.FileName;
      }
    }
  }
}
