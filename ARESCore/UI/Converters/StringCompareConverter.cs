using System;
using System.Globalization;
using System.Windows.Data;

namespace ARESCore.UI.Converters
{
  public class StringCompareConverter : IMultiValueConverter
  {
    public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
    {
      if ( values.Length != 2 )
      {
        return false;
      }

      if ( !( values[0] is string str1 ) || !( values[1] is string str2 ) )
      {
        return false;
      }

      return str1.Equals( str2 );
    }

    public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
    {
      return value as object[];
    }
  }
}
