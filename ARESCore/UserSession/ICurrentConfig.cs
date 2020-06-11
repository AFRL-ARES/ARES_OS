using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.Configurations;

namespace ARESCore.UserSession
{
  public interface ICurrentConfig
  {
    IUserInfo User { get; set; }

    IProjectInfo Project { get; set; }

    IUserSession UserSession { get; set; }

    double TimerPrecision { get; set; }
  }
}
