using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Database.Management
{
  public interface IDbConfigLoader
  {
    IDbConfig Load();
    void Save();
  }
}
