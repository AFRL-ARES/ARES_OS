using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ARESCore.ErrorSupport.Impl;

namespace ARESCore.ErrorSupport.UI
{
  public class SeverityToButtonVisibiltyConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if ( values == null || values.Length != 2 || values[0] == DependencyProperty.UnsetValue)
        return Visibility.Visible;
      ErrorSeverity sev = (ErrorSeverity)values[0];
      string buttonText = (string)values[1];
      if ( buttonText.Equals( "Continue" ) )
      {
        if ( sev == ErrorSeverity.Catastrophic )
          return Visibility.Collapsed;
        if ( sev == ErrorSeverity.Error )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Severe )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Info || sev == ErrorSeverity.Warn)
          return Visibility.Visible;
      }
      else if ( buttonText.Equals( "Retry" ) )
      {
        if ( sev == ErrorSeverity.Catastrophic )
          return Visibility.Collapsed;
        if ( sev == ErrorSeverity.Error )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Severe )
          return Visibility.Collapsed;
        if ( sev == ErrorSeverity.Info || sev == ErrorSeverity.Warn )
          return Visibility.Visible;
      }
      else if ( buttonText.Equals( "Stop" ) )
      {
        if ( sev == ErrorSeverity.Catastrophic )
          return Visibility.Collapsed;
        if ( sev == ErrorSeverity.Error )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Severe )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Info || sev == ErrorSeverity.Warn )
          return Visibility.Visible;
      }
      else if ( buttonText.Equals( "Estop" ) )
      {
        if ( sev == ErrorSeverity.Catastrophic )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Error )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Severe )
          return Visibility.Visible;
        if ( sev == ErrorSeverity.Info || sev == ErrorSeverity.Warn )
          return Visibility.Visible;
      }
      return Visibility.Visible;
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}