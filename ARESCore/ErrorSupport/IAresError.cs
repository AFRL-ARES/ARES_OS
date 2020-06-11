using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ARESCore.ErrorSupport.Impl;
using MahApps.Metro.IconPacks;

namespace ARESCore.ErrorSupport
{
  public interface IAresError
  {
    ErrorSeverity Severity { get; set; }

    string Text { get; set; }

    PackIconMaterialKind Icon { get; set; }

    string DecoratorText { get; set; }
  }
}
