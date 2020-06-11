using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ARESCore.Starter;
using ARESCore.UI.Converters;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Ninject;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for ConsoleWindow.xaml
  /// </summary>
  public partial class ConsoleWindow : MetroWindow
  {
    public ConsoleWindow()
    {
      InitializeComponent();
      ;
      var screenMapper = AresKernel._kernel.Get<IScreenMapper>();
      Left = screenMapper.SecondaryScreen.WorkingArea.Left;
      Top = screenMapper.SecondaryScreen.WorkingArea.Top;
      DataContext = AresKernel._kernel.Get<IAresConsole>();
    }

    private bool AutoScroll = true;

    private void ScrollViewer_ScrollChanged( Object sender, ScrollChangedEventArgs e )
    {
      // User scroll event : set or unset auto-scroll mode
      if ( e.ExtentHeightChange == 0 )
      {   // Content unchanged : user scroll event
        if ( ScrollViewer.VerticalOffset == ScrollViewer.ScrollableHeight )
        {   // Scroll bar is in bottom
          // Set auto-scroll mode
          AutoScroll = true;
        }
        else
        {   // Scroll bar isn't in bottom
          // Unset auto-scroll mode
          AutoScroll = false;
        }
      }

      // Content scroll event : auto-scroll eventually
      if ( AutoScroll && e.ExtentHeightChange != 0 )
      {   // Content changed and auto-scroll mode set
        // Autoscroll
        ScrollViewer.ScrollToVerticalOffset( ScrollViewer.ExtentHeight );
      }
    }

    private void ScrollViewer_PreviewMouseWheel( object sender, System.Windows.Input.MouseWheelEventArgs e )
    {
      ScrollViewer scv = (ScrollViewer)sender;
      scv.ScrollToVerticalOffset( scv.VerticalOffset - e.Delta );
      e.Handled = true;
    }

    private void ConsoleWindowClosed( object sender, EventArgs e )
    {
      Application.Current.Shutdown();
    }

    private void ConsoleWindowOnLoaded( object sender, RoutedEventArgs e )
    {
      this.Icon = IconConverter.Convert( PackIconMaterialKind.Console );
    }
  }
}
