using ARESCore.UI.Converters;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ARESCore.UserSession
{
  /// <summary>
  /// Interaction logic for UserSessionSelection.xaml
  /// </summary>

  public partial class UserSessionSelection : MetroWindow
  {
    public UserSessionSelection()
    {
      InitializeComponent();
    }

    private void SetWorkingDirectory_OnClick(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        (DataContext as UserSessionSelectionViewModel).WorkingDirectory = fbd.SelectedPath;
      }
    }

    private void UserSessionSelectionLoaded(object sender, RoutedEventArgs e)
    {
      Icon = IconConverter.Convert(PackIconMaterialKind.LoginVariant);
    }

    private void UserSessionSelection_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if ((DataContext as UserSessionSelectionViewModel).UserIsSelected && e.Key == Key.Enter)
      {
        loadButton.Command.Execute(null);
      }
    }
  }
}