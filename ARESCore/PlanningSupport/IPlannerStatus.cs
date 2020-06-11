using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ARESCore.PlanningSupport
{
  public interface IPlannerStatus
  {
    string StatusText { get; set; }

    ContentControl Image { get; set; }
  }
}
