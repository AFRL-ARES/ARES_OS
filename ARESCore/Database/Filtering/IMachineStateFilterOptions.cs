using System;
using System.Collections.Generic;

namespace ARESCore.Database.Filtering
{
  public interface IMachineStateFilterOptions : IFilterOptions
  {
    bool FilterChipDescriptions { get; set; }
    IList<string> Chips { get; set; }
  }
}
