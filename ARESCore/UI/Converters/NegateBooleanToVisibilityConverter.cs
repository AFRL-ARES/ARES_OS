using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ARESCore.UI.Converters
{
  public sealed class NegateBooleanToVisibilityConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      bool flag = false;
      if ( value is bool )
        flag = (bool)value;
      return flag ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      return value as Visibility? == Visibility.Collapsed;
    }
  }
}
