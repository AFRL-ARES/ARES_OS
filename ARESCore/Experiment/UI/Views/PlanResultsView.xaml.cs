using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ARESCore.Experiment.UI.ViewModels;

namespace ARESCore.Experiment.UI.Views
{
  /// <summary>
  /// Interaction logic for PlanResultsView.xaml
  /// </summary>
  public partial class PlanResultsView : UserControl
  {
    private PlanResultsViewModel _viewModel;

    public PlanResultsView()
    {
      InitializeComponent();
      _viewModel = DataContext as PlanResultsViewModel;
      _viewModel.PlannerColumns.CollectionChanged += ( sender, args ) => GenerateGrid();
    }


    private void PopulateGrid( object sender, DependencyPropertyChangedEventArgs e )
    {
      if ( ( (FrameworkElement)sender ).Visibility == Visibility.Visible )
      {
        GenerateGrid();
      }
      else
      {
        _planGridView.Columns.Clear();
      }
    }

    private void GenerateGrid()
    {
      Application.Current.Dispatcher.BeginInvoke( new Action( () =>
      {
        _planGridView.Columns.Clear();
        for ( int i = 0; i < _viewModel.PlannerColumns.Count; i++ )
        {
          _planGridView.Columns.Add( new GridViewColumn
          {
            Header = _viewModel.PlannerColumns[i],
            DisplayMemberBinding = new Binding( $"[{i}]" ),
            Width = double.NaN
          } );
        }
      } ) );
    }
  }
}

