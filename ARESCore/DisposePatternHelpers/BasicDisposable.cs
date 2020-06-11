using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace ARESCore.DisposePatternHelpers
{
  /// <summary>
  /// Abstract class which implements <see cref="IBasicDisposable"/> and provides a 
  /// container for adding <see cref="IBasicDisposable"/>s to that will be cleaned up when disposed.
  /// </summary>
  public abstract class BasicDisposable : IBasicDisposable
  {
    private CompositeDisposable _disposables = new CompositeDisposable();

#if DISPOSAL

    private readonly DisposableDebugger _debugger = new DisposableDebugger();

    ~BasicDisposable()
    {
      _debugger.OnExit();
    }

#endif

    /// <summary>
    /// A <see cref="CompositeDisposable"/> container to manage <see cref="IDisposable"/> instances.
    /// Calling <see cref="Dispose"/> will dispose and then set this object to <code>null</code>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot set the value of <see cref="Disposables"/> after <see cref="Dispose"/> is called.</exception>
    protected CompositeDisposable Disposables
    {
      get { return _disposables; }
      set
      {
        if ( IsDisposed )
          throw new InvalidOperationException( $"Cannot set {nameof( Disposables )} after {nameof( Dispose )} is called" );
        _disposables = value;
      }
    }

    /// <summary>
    /// Value used to show if <see cref="Dispose"/> has been called.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Performs application-defined task associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
      if ( IsDisposed )
        return;
      IsDisposed = true;
      Disposables.Dispose();
      _disposables = null;
      GC.SuppressFinalize( this );
    }
  }
}
