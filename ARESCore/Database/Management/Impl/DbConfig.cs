using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Database.Management.Impl
{
  public class DbConfig: IDbConfig
  {
    public string Ip { get; set; }

    public int Port { get; set; }
  }
}
