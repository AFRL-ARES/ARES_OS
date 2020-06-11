using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ARESCore.UI.ViewModels;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for AresNotification.xaml
  /// </summary>
  public partial class AresNotifier : UserControl
  {
    public AresNotifier()
    {
      InitializeComponent();
    }

    private void ButtonBase_OnClick( object sender, RoutedEventArgs e )
    {
      ( DataContext as AresNotifierViewModel ).FinishInteraction();
    }
  }
}
