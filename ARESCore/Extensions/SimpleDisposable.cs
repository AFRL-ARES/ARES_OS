using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Extensions
{
  public class SimpleDisposable : BasicReactiveObjectDisposable
  {
    private readonly Stack<IDisposable> _disposable;

    public SimpleDisposable()
    {
      _disposable = new Stack<IDisposable>();
    }

    public SimpleDisposable( params IDisposable[] disposal )
    {
      _disposable = new Stack<IDisposable>( disposal );
    }

    public SimpleDisposable( params Action[] disposal ) : this()
    {
      AddRange( disposal );
    }

    public void Add( IDisposable disposable )
    {
      _disposable.Push( disposable );
    }

    public void Add( Action disposableAction )
    {
      _disposable.Push( Disposable.Create( disposableAction ) );
    }

    public void AddRange( params Action[] disposals )
    {
      foreach ( var disposal in disposals )
        Add( disposal );
    }

    public void AddRange( params IDisposable[] disposals )
    {
      foreach ( var disposal in disposals )
        Add( disposal );
    }

    private readonly object _lock = new object();

    public void Dispose()
    {
      IDisposable[] disposableArray;
      lock ( _lock )
      {
        disposableArray = _disposable.ToArray();
        _disposable.Clear();
      }
      foreach ( IDisposable disposable in disposableArray )
      {
        disposable.Dispose();
      }
    }
  }
}
