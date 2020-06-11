using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Configurations
{
  public interface IUserDeviceConfig : IBasicReactiveObjectDisposable
  {
    void Load();
    void Save();
  }
}
