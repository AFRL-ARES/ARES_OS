using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ARESCore.Configurations;
using ARESCore.Database.ViewModels;

namespace ARESCore.Database.Views
{
  /// <summary>
  /// Interaction logic for FilterTabView.xaml
  /// </summary>
  public partial class FilterTabView : UserControl
  {
    public FilterTabView()
    {
      InitializeComponent();
    }

    private void ListViewSelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      var vm = (FilterTabViewModel)DataContext;
      vm.SelectedProjects = _listView.SelectedItems.Cast<IProjectInfo>().ToList();
    }
  }
}
