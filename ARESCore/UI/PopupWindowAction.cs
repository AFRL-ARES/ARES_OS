using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using MahApps.Metro.Controls;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace ARESCore.UI
{
  public class MahAppsPopupWindowAction : PopupWindowAction
  {
    const int WM_SYSCOMMAND = 0x0112;
    const int SC_CLOSE = 0xF060;

    public static readonly DependencyProperty SuppressUserClosingProperty =
      DependencyProperty.Register(
        "SuppressUserClosing",
        typeof( bool ),
        typeof( MahAppsPopupWindowAction ),
        new PropertyMetadata( null ) );

    public bool SuppressUserClosing
    {
      get { return (bool)GetValue( SuppressUserClosingProperty ); }
      set { SetValue( SuppressUserClosingProperty, value ); }
    }

    protected MetroWindow metroWindow
    {
      get
      {
        var mWindow = Window.GetWindow( AssociatedObject ) as MetroWindow;
        if ( mWindow == null )
        {
          throw new InvalidOperationException( "Context is not inside a MetroWindow." );
        }
        return mWindow;
      }
    }

    private IntPtr UserClosingWndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
    {
      switch ( msg )
      {
        case WM_SYSCOMMAND:
          int command = wParam.ToInt32() & 0xfff0;
          if ( command == SC_CLOSE )
            handled = true;
          break;
        default:
          break;
      }
      return IntPtr.Zero;
    }

    protected override Window GetWindow( INotification notification )
    {
      var mwin = base.GetWindow( notification );

      metroWindow.ShowOverlay();

      HwndSource hwndSource = null;
      if ( SuppressUserClosing )
      {
        RoutedEventHandler loadedHandler = null;
        loadedHandler = ( o, e ) =>
        {
          mwin.Loaded -= loadedHandler;

          hwndSource = HwndSource.FromHwnd( new WindowInteropHelper( mwin ).Handle );
          hwndSource.AddHook( UserClosingWndProc );
        };
        mwin.Loaded += loadedHandler;
      }

      EventHandler closeWindowHandler = null;
      closeWindowHandler =
        ( o, e ) =>
        {
          mwin.Closed -= closeWindowHandler;
          if ( hwndSource != null )
          {
            hwndSource.RemoveHook( UserClosingWndProc );
            hwndSource.Dispose();
          }

          // hide overlay
          metroWindow.HideOverlayAsync();
        };
      mwin.Closed += closeWindowHandler;

      return mwin;
    }

    protected override Window CreateWindow()
    {
      var mwin = new MetroWindow
      {
        Owner = metroWindow,
        ShowInTaskbar = false,
        ShowActivated = true,
        Topmost = false,
        ResizeMode = ResizeMode.NoResize,
        WindowStyle = System.Windows.WindowStyle.None,
        WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
        ShowTitleBar = false,
        ShowCloseButton = false,
        WindowTransitionsEnabled = false,
        Width = metroWindow.ActualWidth,
        MinHeight = SystemParameters.PrimaryScreenHeight / 4.0,
        MaxHeight = metroWindow.ActualHeight,
        SizeToContent = SizeToContent.Height
      };

      return mwin;
    }
  }
}
