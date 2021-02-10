using System.Collections;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Database.Filtering
{
  public interface IDbFilterManager: IReactiveSubscriber
  {
    void DoFilter();

    IEnumerable LastFilterResult { get; set; }

  }
}
