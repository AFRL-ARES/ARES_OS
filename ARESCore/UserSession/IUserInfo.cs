using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ARESCore.UserSession
{
  public interface IUserInfo
  {
    string Username { get; set; }
    string SaveFileName { get; set; }
    string SaveDirectory { get; set; }
    DateTime LastLoadedDate { get; set; }
  }
}
