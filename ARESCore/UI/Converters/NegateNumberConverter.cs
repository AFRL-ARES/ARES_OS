using System;
using System.Globalization;
using System.Windows.Data;

namespace ARESCore.UI.Converters
{
  public class NegateNumberConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return 0;
      }
      if (value is double d)
      {
        return -d;
      }
      if (value is float f)
      {
        return -f;
      }
      if (value is int i)
      {
        return -i;
      }
      if (value is short s)
      {
        return -s;
      }
      if (value is byte b)
      {
        return -b;
      }
      return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return 0;
      }
      if (value is double d)
      {
        return +d;
      }
      if (value is float f)
      {
        return +f;
      }
      if (value is int i)
      {
        return +i;
      }
      if (value is short s)
      {
        return +s;
      }
      if (value is byte b)
      {
        return +b;
      }
      return 0;
    }
  }
}
