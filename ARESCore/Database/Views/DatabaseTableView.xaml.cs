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
using ARESCore.Database.ViewModels;

namespace ARESCore.Database.Views
{
  /// <summary>
  /// Interaction logic for DatabaseTableView.xaml
  /// </summary>
  public partial class DatabaseTableView : UserControl
  {
    private readonly DatabaseTableViewModel _viewModel;

    public DatabaseTableView()
    {
      InitializeComponent();
      _viewModel = DataContext as DatabaseTableViewModel;
      _viewModel.DatabaseColumns.CollectionChanged += ( sender, args ) => GenerateGrid();
    }


    private void PopulateGrid( object sender, DependencyPropertyChangedEventArgs e )
    {
      if ( ( (FrameworkElement)sender ).Visibility == Visibility.Visible )
      {
        GenerateGrid();
      }
      else
      {
        _databaseGridView.Columns.Clear();
      }
    }

    private void GenerateGrid()
    {
      Application.Current.Dispatcher.BeginInvoke( new Action( () =>
      {
        _databaseGridView.Columns.Clear();
        for ( int i = 0; i < _viewModel.DatabaseColumns.Count; i++ )
        {
          _databaseGridView.Columns.Add( new GridViewColumn
          {
            Header = _viewModel.DatabaseColumns[i],
            DisplayMemberBinding = new Binding( $"[{i}]" ),
            Width = double.NaN
          } );
        }
      } ) );
    }
  }
}
