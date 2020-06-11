using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ARESCore.Extensions
{
  public static class ScrollViewerExtensions
  {
    public static DependencyProperty IgnoreWheelScrollProperty = DependencyProperty.RegisterAttached(
      "IgnoreWheelScroll",
      typeof(bool),
      typeof(ScrollViewerExtensions),
      new UIPropertyMetadata(false, IgnoreWheelScrollChanged));

    public static bool GetIgnoreWheelScroll(DependencyObject dependencyObj)
    {
      return (bool)dependencyObj.GetValue(IgnoreWheelScrollProperty);
    }

    public static void SetIgnoreWheelScroll(DependencyObject dependencyObj, bool value)
    {
      dependencyObj.SetValue(IgnoreWheelScrollProperty, value);
    }

    public static void IgnoreWheelScrollChanged(DependencyObject dependencyObj, DependencyPropertyChangedEventArgs eventArgs)
    {
      var newVal = (bool)eventArgs.NewValue;
      var oldVal = (bool)eventArgs.OldValue;

      var frameworkElement = dependencyObj as FrameworkElement;
      if (frameworkElement == null)
      {
        return;
      }

      if (!newVal || oldVal || frameworkElement.IsFocused)
      {
        return;
      }

      var scrollViewer = frameworkElement as ScrollViewer;
      if (scrollViewer == null)
      {
        return;
      }

      scrollViewer.PreviewMouseWheel += ScrollViewerOnPreviewMouseWheel;
    }

    private static void ScrollViewerOnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (!(sender is ScrollViewer) || e.Handled)
      {
        return;
      }
      e.Handled = true;

      var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
      {
        RoutedEvent = UIElement.MouseWheelEvent,
        Source = sender
      };

      var parent = ((Control)sender).Parent as UIElement;
      if (parent != null)
      {
        parent.RaiseEvent(eventArg);
      }
    }
  }
}
