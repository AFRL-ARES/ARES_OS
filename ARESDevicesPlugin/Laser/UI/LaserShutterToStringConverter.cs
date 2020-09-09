using System;
using System.Globalization;
using System.Windows.Data;

namespace AresCNTDevicesPlugin.Laser.UI
{
  public class LaserShutterToStringConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      if ( !( value is bool ) )
        return "Open Shutter";
      if ((bool)value)
        return "Close Shutter";
      return "Open Shutter";
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
