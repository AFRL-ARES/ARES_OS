using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.DataHub.impl
{
  public class DataHub : BasicReactiveObjectDisposable, IDataHub
  {
    private IDataHubEntry _data;

    public IDataHubEntry Data
    {
      get => _data;
      set
      { //changing it because it will only notify if the value changes. It needs to notify regardless. NO IFS,ANDS, or BUTS. YOU DO IT!!
        _data = value;
        this.RaisePropertyChanged("Data");
      }
    }
  }
}