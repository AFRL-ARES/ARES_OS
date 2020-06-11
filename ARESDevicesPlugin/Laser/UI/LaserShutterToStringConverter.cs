using System;
using System.Globalization;
using System.Windows.Data;

namespace ARESDevicesPlugin.Laser.UI
{
  public class LaserShutterToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool))
        return "Open Shutter";
      var bval = value as bool?;
      if (bval.Value)
        return "Close Shutter";
      return "Open Shutter";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
