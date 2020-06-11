using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.DisposePatternHelpers
{
  internal class DisposableDebugger
  {
    public string Stack;
    public static uint Count;

    public DisposableDebugger()
    {
      Stack = Environment.StackTrace;
      Count++;
    }

    public void OnExit()
    {
      throw new InvalidOperationException( "Destroyed without being disposed" );
    }
  }
}
