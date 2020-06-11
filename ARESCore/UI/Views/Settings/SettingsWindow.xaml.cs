using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ARESCore.UI.Converters;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

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

    private void SettingsWindowLoaded( object sender, RoutedEventArgs e )
    {
      this.Icon = IconConverter.Convert( PackIconMaterialKind.Settings );
    }
  }
}
