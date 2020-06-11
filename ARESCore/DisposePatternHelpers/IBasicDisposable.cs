using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.DisposePatternHelpers
{
  /// <summary>
  /// Interface definition for classes which must implement <see cref="IDisposable"/>. Rather than simply using
  /// the built in <see cref="IDisposable"/> interface, this is provided to encourage the use of <see cref="BasicDisposable"/>
  /// and <see cref="BasicReactiveObjectDisposable"/> abstract classes which implement <see cref="IDisposable"/> 
  /// properly and provide additional debugging assistance.
  /// </summary>
  public interface IBasicDisposable : IDisposable
  {

  }
}