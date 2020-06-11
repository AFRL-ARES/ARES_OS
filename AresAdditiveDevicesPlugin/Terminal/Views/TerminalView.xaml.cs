using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace AresAdditiveDevicesPlugin.Terminal.Views
{
  /// <summary>
  /// Interaction logic for TerminalView.xaml
  /// </summary>
  public partial class TerminalView : UserControl
  {

    public TerminalView()
    {
      InitializeComponent();
    }



    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Return || e.Key == Key.Enter)
      {
        var enteredText = (sender as TextBox).Text;
        (DataContext as ITerminal)?.WriteLine($"{enteredText}");
        (DataContext as ITerminal)?.SendMessage(enteredText);

        ((TextBox)sender).Text = string.Empty;
      }
    }

    private bool AutoScroll = true;

    private void ScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
    {
      // User scroll event : set or unset auto-scroll mode
      if (e.ExtentHeightChange == 0)
      {   // Content unchanged : user scroll event
        if (_scrollViewer.VerticalOffset == _scrollViewer.ScrollableHeight)
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
      if (AutoScroll && e.ExtentHeightChange != 0)
      {   // Content changed and auto-scroll mode set
          // Autoscroll
        _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ExtentHeight);
      }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      ScrollViewer scv = (ScrollViewer)sender;
      scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
      e.Handled = true;
    }
  }
}
