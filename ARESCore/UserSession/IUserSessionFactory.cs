using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARESCore.UserSession
{
  public interface IUserSessionFactory
  {
    IUserSession CreateSession(string userPath);
  }
}
