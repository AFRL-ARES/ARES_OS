using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ARESCore.UI.Converters
{
  public class EnumBooleanConverter : IValueConverter
  {
    #region IValueConverter Members
    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {


      if ( Enum.IsDefined( value.GetType(), value ) == false )
        return DependencyProperty.UnsetValue;

      return parameter.Equals( value );
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      return parameter;
    }
    #endregion
  }
}
