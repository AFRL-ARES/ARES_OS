using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ARESCore.UI.Converters;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for SettingsWindow.xaml
  /// </summary>
  public partial class LicenseWindow : MetroWindow
  {
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
      Close();
    }
    public LicenseWindow()
    {
      InitializeComponent();
    }
  }
}
