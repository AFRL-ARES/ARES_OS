using ARESCore.UI.Converters;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System.Windows;

namespace ARESCore.UI.Views.Settings
{
  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : MetroWindow
  {
    public SettingsWindow()
    {
      InitializeComponent();
    }

    private void SettingsWindowLoaded(object sender, RoutedEventArgs e)
    {
      this.Icon = IconConverter.Convert(PackIconMaterialKind.ApplicationSettings);
    }
  }
}
