using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Database.Tables
{
  public interface IMachineState
  {
    string JsonTitle { get; set; }
    void TryParse( string currentDesc, string lineToken, bool tokenHasValue );

    IMachineState Clone();
  }
}
