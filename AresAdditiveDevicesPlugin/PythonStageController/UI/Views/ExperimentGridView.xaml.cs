using System.Windows.Controls;
using System.Windows.Input;
using AresAdditiveDevicesPlugin.PythonStageController.UI.Vms;

namespace AresAdditiveDevicesPlugin.PythonStageController.UI.Views
{
  /// <summary>
  /// Interaction logic for ExperimentGridView.xaml
  /// </summary>
  public partial class ExperimentGridView : UserControl
  {
    public ExperimentGridView()
    {
      InitializeComponent();
    }

    private void ToggleExperimentValidity(object sender, MouseButtonEventArgs e)
    {
      var button = (Button)sender;
      var experimentCell = (int)button.Tag;
      var experimentCellAvailability = ((ExperimentGridViewModel)DataContext).StageController.ExperimentGrid[experimentCell];
      ((ExperimentGridViewModel)DataContext).StageController.ExperimentGrid[experimentCell] = !experimentCellAvailability;
    }
  }
}
