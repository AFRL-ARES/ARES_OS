using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.DisposePatternHelpers;
using Ninject;
using ReactiveUI;
using System.Collections;
using System.Threading.Tasks;

namespace ARESCore.Database.Filtering.Impl
{
  internal class DbFilterManager : BasicReactiveObjectDisposable, IDbFilterManager
  {
    private readonly IDbFilter<ExperimentEntity> _expFilter;
    private readonly IDbFilter<DataEntity> _dataFilter;
    private IEnumerable _lastFilterResult;
    private readonly IDbFilter<MachineStateEntity> _machineStateFilter;

    public DbFilterManager(IDbFilter<ExperimentEntity> expFilter)
    {
      _expFilter = expFilter;
      _dataFilter = AresKernel._kernel.TryGetAndThrowOnInvalidBinding<IDbFilter<DataEntity>>();
      _machineStateFilter = AresKernel._kernel.TryGetAndThrowOnInvalidBinding<IDbFilter<MachineStateEntity>>();
    }

    public async void DoFilter()
    {
      var startExpDb = AresKernel._kernel.Get<AresContext>().Experiments;
      var startDataDb = AresKernel._kernel.Get<AresContext>().Data;
      var startMachStateDb = AresKernel._kernel.Get<AresContext>().MachineStates;
      await Task.Run(() =>
     {
       var expFilteredDb = _expFilter.Filter(startExpDb);
       var expDataFilteredDb = _dataFilter.FilterExperimentsOn(expFilteredDb, startDataDb);
       if (_machineStateFilter != null)
       {
         var expMachStateFilteredDb = _machineStateFilter.FilterExperimentsOn(expDataFilteredDb, startMachStateDb);
         LastFilterResult = expMachStateFilteredDb;
       }
       else
       {
         LastFilterResult = expDataFilteredDb;
       }
     });
    }

    public IEnumerable LastFilterResult
    {
      get => _lastFilterResult;
      set => this.RaiseAndSetIfChanged(ref _lastFilterResult, value);
    }
  }
}