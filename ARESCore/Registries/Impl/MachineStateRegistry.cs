using System;
using System.Reactive.Linq;
using ARESCore.Database.Tables;
using DynamicData.Binding;

namespace ARESCore.Registries.Impl
{
  internal class MachineStateRegistry : ObservableCollectionExtended<IMachineState>, IMachineStateRegistry
  {
    private int _msBetweenWrites = 5000;
    private IDisposable _subscription;

    public int MsBetweenWrites
    {
      get => _msBetweenWrites;
      set
      {
        _msBetweenWrites = value;
        InitializeWriter();
      }
    }

    private void InitializeWriter()
    {
      _subscription?.Dispose();
      _subscription = Observable.Interval( TimeSpan.FromMilliseconds( MsBetweenWrites ) ).Subscribe( _ => CollectEntries() );
    }

    private void CollectEntries()
    {
      foreach ( var state in this )
      {
        // TODO FIXME
      }
    }

    public MachineStateRegistry()
    {
      InitializeWriter();
    }
  }
}