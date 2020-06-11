using System.Windows.Controls;
using System.Windows.Input;

namespace ARESCore.Experiment.UI.Views
{
  /// <summary>
  /// Interaction logic for BatchExecutionView.xaml
  /// </summary>
  public partial class CampaignExecutionView : UserControl
  {
    private bool _autoScroll;

    public CampaignExecutionView()
    {
      InitializeComponent();
    }

    private void ScrollViewer_OnScrollChanged( object sender, ScrollChangedEventArgs e )
    {
      var scrollViewer = (ScrollViewer)sender;

      // User scroll event : set or unset auto-scroll mode
      if ( e.ExtentHeightChange == 0 )
      {   // Content unchanged : user scroll event
        if ( scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight )
        {   // Scroll bar is in bottom
          // Set auto-scroll mode
          _autoScroll = true;
        }
        else
        {   // Scroll bar isn't in bottom
          // Unset auto-scroll mode
          _autoScroll = false;
        }
      }

      // Content scroll event : auto-scroll eventually
      if ( _autoScroll && e.ExtentHeightChange != 0 )
      {   // Content changed and auto-scroll mode set
        // Autoscroll
        scrollViewer.ScrollToVerticalOffset( scrollViewer.ExtentHeight );
      }
    }

    private void ScrollViewer_OnPreviewMouseWheel( object sender, MouseWheelEventArgs e )
    {
      var scv = (ScrollViewer)sender;
      scv.ScrollToVerticalOffset( scv.VerticalOffset - e.Delta );
      e.Handled = true;
    }
  }
}
