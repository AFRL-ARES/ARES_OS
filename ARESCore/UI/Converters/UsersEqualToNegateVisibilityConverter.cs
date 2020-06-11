using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ARESCore.UserSession;

namespace ARESCore.UI.Converters
{
  public class UsersEqualToNegateVisibilityConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if ( values.Length != 2 )
        return Visibility.Collapsed;
      if(values[0] is IUserInfo && values[1] is IUserInfo)
      {
        var info1 = (IUserInfo)values[0];
        var info2 = (IUserInfo)values[1];
        return info2.Equals( info1 ) ? Visibility.Collapsed: Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}