using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for AppKiller.xaml
  /// </summary>
  public partial class AppKiller : MetroWindow
  {
    public AppKiller()
    {
      InitializeComponent();
    }

    protected override void OnContentRendered( EventArgs e )
    {
      Close();
      Application.Current.Shutdown();
    }
  }
}
