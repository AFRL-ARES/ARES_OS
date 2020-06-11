using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace ARESCore.ErrorSupport.Impl
{
  public class AresError: IAresError
  {
    public ErrorSeverity Severity { get; set; }
    public string Text { get; set; }
    public PackIconMaterialKind Icon { get; set; }
    public string DecoratorText { get; set; }
  }
}
