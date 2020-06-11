using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Converters
{
    public class MultiNegateBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (value is bool && (bool)value)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
