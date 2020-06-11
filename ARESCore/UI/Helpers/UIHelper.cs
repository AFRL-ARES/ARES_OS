using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ARESCore.UI.Helpers
{
  public static class UIHelper
  {
    public static T FindAncestorOrSelf<T>( DependencyObject obj ) where T : DependencyObject
    {
      while ( obj != null )
      {
        T objTest = obj as T;
        if ( objTest != null )
          return objTest;
        obj = VisualTreeHelper.GetParent( obj );
      }
      return null;
    }
  }
}