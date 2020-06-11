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
  public class NegateBooleanConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      bool flag = true;
      if ( value is bool )
        flag = (bool)value;
      return !flag;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      bool flag = true;
      if ( value is bool )
        flag = (bool)value;
      return !flag;
    }
  }
}
