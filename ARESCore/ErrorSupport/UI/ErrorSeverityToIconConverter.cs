using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ARESCore.ErrorSupport.Impl;
using MahApps.Metro.IconPacks;

namespace ARESCore.ErrorSupport.UI
{
  public class ErrorSeverityToIconConverter : IValueConverter
  {
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
    {
      ErrorSeverity sev = (ErrorSeverity)value;
      if ( sev == ErrorSeverity.Catastrophic )
        return PackIconMaterialKind.Fire;
      if ( sev == ErrorSeverity.Error )
        return PackIconMaterialKind.AlertCircleOutline;
      if ( sev == ErrorSeverity.Severe )
        return PackIconMaterialKind.HeartBroken;
      if ( sev == ErrorSeverity.Warn )
        return PackIconMaterialKind.ClipboardAlert;
        return PackIconMaterialKind.Note;
    }

    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}