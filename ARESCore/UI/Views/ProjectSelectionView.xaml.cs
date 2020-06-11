using System.Windows;
using System.Windows.Forms;
using ARESCore.UI.Converters;
using ARESCore.UI.ViewModels;
using ARESCore.UserSession;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for ProjectSelectionView.xaml
  /// </summary>
  public partial class ProjectSelectionView : MetroWindow
  {
    public ProjectSelectionView()
    {
      InitializeComponent();
    }

    private void ProjectSelectionViewLoaded( object sender, RoutedEventArgs e )
    {
      Icon = IconConverter.Convert( PackIconMaterialKind.BookOpenVariant );
    }

    private void NewProjectClick( object sender, RoutedEventArgs e )
    {
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      if ( fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK )
      {
        ( DataContext as ProjectSelectionViewModel ).NewProjectWorkingDirectory = fbd.SelectedPath;
      }
    }
  }
}
