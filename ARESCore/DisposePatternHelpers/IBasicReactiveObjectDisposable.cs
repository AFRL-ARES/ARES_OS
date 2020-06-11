using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.ErrorSupport;
using ReactiveUI;

namespace ARESCore.DisposePatternHelpers
{
  /// <summary>
  /// Interface definition for classes which must implement both <see cref="IReactiveObject"/> and <see cref="IDisposable"/>.
  /// </summary>
  public interface IBasicReactiveObjectDisposable : IReactiveObject, IBasicDisposable, IErrorable
  {
  }
}
