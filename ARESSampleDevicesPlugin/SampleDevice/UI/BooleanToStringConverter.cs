using System;
using System.Globalization;
using System.Windows.Data;

namespace AresSampleDevicesPlugin.SampleDevice.UI
{
  public class BooleanToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool))
        return "Make this True";
      if ((bool)value)
        return "Make this False";
      return "Make this True";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
