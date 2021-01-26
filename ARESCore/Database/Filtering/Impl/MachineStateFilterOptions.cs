using System.Collections.Generic;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Database.Filtering.Impl
{
  public class MachineStateFilterOptions : ReactiveSubscriber, IMachineStateFilterOptions
  {
    public bool FilterChipDescriptions { get; set; }
    public IList<string> Chips { get; set; } = new List<string>();
  }
}
