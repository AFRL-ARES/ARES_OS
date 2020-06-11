using System.Collections;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Database.Filtering
{
  public interface IDbFilterManager: IBasicReactiveObjectDisposable
  {
    void DoFilter();

    IEnumerable LastFilterResult { get; set; }

  }
}
