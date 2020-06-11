using System;
using System.Globalization;
using System.Windows.Data;

namespace ARESCore.UI.Converters
{
  public class DoubleAdditionConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
      {
        return 0;
      }
      var addition = double.Parse(parameter.ToString());
      if (value is int i)
      {
        var newVal = i + addition;
        return newVal;
      }
      if (value is double d)
      {
        return d + addition;
      }
      return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
