
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Database.Management
{
  public interface IDbConfig
  {
    string Ip { get; set; }

    int Port { get; set; }
  }
}