using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Configurations
{
  public interface IAppDeviceConfig : IBasicReactiveObjectDisposable
  {
    void Load();
    void Save();
  }
}
