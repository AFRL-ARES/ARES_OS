using AresAdditiveDevicesPlugin.Processing.Components;
using CommonServiceLocator;
using DynamicData.Binding;
using MoreLinq;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace AresAdditiveDevicesPlugin.PythonInterop.Impl
{
  public class PythonProcessRepository : ObservableCollectionExtended<IPythonProcess>, IPythonProcessRepository
  {
    public PythonProcessRepository()
    {
      CollectionChanged += HandleCollectionChanged;
    }

    private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        e.NewItems.Cast<IPythonProcess>().ForEach(AddProcess);
      }
    }

    private void AddProcess(IPythonProcess item)
    {
      Application.Current.Dispatcher.Invoke(delegate
      {
        var vmsvc = ServiceLocator.Current.GetInstance<IComponentService>();
        vmsvc.Add(item);
      });
    }
  }
}
