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
  public class NullToBoolConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if(value == null)
      return false;
      return true;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      return true;
    }
  }
}
