using ARESCore.DisposePatternHelpers;
using Prism.Common;

namespace ARESCore.DataHub
{
  public interface IDataHub : IReactiveSubscriber
  {
    IDataHubEntry Data { get; set; }
   // void PutData(IDataHubEntry dataHubEntry);
  }
}
