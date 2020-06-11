using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARESCore.ErrorSupport
{
  public interface IErroredBundle
  {
    IErrorable ErrorHandler { get; set; }

    IErrorable ErrorNotifier { get; set; }
  }
}