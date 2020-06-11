using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ARESCore.UI.Converters;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

namespace ARESCore.Database.Views
{
  /// <summary>
  /// Interaction logic for DatabaseView.xaml
  /// </summary>
  public partial class DatabaseView : MetroWindow
  {
    public DatabaseView()
    {
      InitializeComponent();
    }

    private void DatabaseViewLoaded( object sender, RoutedEventArgs e )
    {
      Icon = IconConverter.Convert( PackIconMaterialKind.Database );
    }

    private void ButtonBase_OnClick( object sender, RoutedEventArgs e )
    {
      this.Close();
    }
  }
}
