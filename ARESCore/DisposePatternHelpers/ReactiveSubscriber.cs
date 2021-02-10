using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using ARESCore.ErrorSupport;
using ARESCore.ErrorSupport.Impl;
using ReactiveUI;

namespace ARESCore.DisposePatternHelpers
{
  /// <summary>
  /// Abstract class that inherits from <see cref="ReactiveObject"/>  and implements <see cref="IReactiveSubscriber"/>.
  /// This also provides a container for adding <see cref="IDisposable"/>s to that will be cleaned up when disposed.
  /// </summary>
  public abstract class ReactiveSubscriber : Errorable, IReactiveSubscriber
  {
    private IDisposable _disposal = Disposable.Empty;
    private CompositeDisposable _disposables = new CompositeDisposable();

#if DISPOSAL

    private readonly DisposableDebugger _debugger = new DisposableDebugger();

    ~ReactiveSubscriber()
    {
      _debugger.OnExit();
    }

#endif

    /// <summary>
    /// An <see cref="IDisposable"/> which, on <see cref="Dispose"/>, will dispose and then be set to <code>null</code>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Cannot set the value of <see cref="Disposal"/> after <see cref="Dispose"/> is called.</exception>
    protected IDisposable Disposal
    {
      get { return _disposal; }
      set
      {
        if ( IsDisposed )
          throw new InvalidOperationException( $"Cannot set {nameof( Disposal )} after {nameof( Dispose )} is called" );
        _disposal = value;
      }
    }

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
      Disposal.Dispose();
      Disposables.Dispose();
      _disposables = null;
      _disposal = null;
      GC.SuppressFinalize( this );
    }
  }
}
