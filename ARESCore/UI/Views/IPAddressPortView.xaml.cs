using ARESCore.UI.Helpers;
using MahApps.Metro.Controls;
using System.Windows;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for IPAddressPortView.xaml
  /// </summary>
  public partial class IPAddressPortView : MetroWindow, IClosable
  {
    public IPAddressPortView()
    {
      InitializeComponent();
      WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private void OkClicked(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      this.Close();
    }
  }
}