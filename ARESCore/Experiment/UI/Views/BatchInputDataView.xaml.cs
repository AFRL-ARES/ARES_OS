using System.Windows;
using ARESCore.Database.Views;
using ARESCore.Experiment.UI.ViewModels;
using Ninject;

namespace ARESCore.Experiment.UI.Views
{
  /// <summary>
  /// Interaction logic for BatchInputDataView.xaml
  /// </summary>
  public partial class BatchInputDataView : System.Windows.Controls.UserControl
  {
    private readonly BatchInputDataViewModel _viewModel;

    public BatchInputDataView()
    {
      InitializeComponent();
      _viewModel = AresKernel._kernel.Get<BatchInputDataViewModel>();
    }


    private void GenerateDbClick(object sender, RoutedEventArgs e)
    {
      var expType = _viewModel.SelectedExperiment;
      var dbViewer = new DatabaseView(); //( expType);
      var dr = dbViewer.ShowDialog();
      if (dr.HasValue && dr.Value)
      {
        // _viewModel.UpdateDbOptions( dbViewer.Vm );
      }

      /*var dbCreator = new PlanningDatabaseCreator(SelectedBatchType, PlannerTypes.BORAAS);
      if (dbCreator.ShowDialog() == DialogResult.OK)
      {
          SharedDatabaseCreated = true;
          SharedPlanningDatabase = dbCreator.PlannerDatabase;
          SharedPlannerFilterOptions = dbCreator.PlannerFilterOptions;
      }*/
    }
  }
}