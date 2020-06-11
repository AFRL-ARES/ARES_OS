using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScreenHelper;

namespace ARESCore.Starter
{
  public interface IScreenMapper
  {
    Screen PrimaryScreen { get; set; }

    Screen SecondaryScreen { get; set; }

    Screen TopScreen { get; set; }
  }
}
