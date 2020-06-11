using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables.InnerContent
{
  public class ExperimentExecutionResults
  {
    [CanBeNull]
    public TimeSpan Runtime { get; set; }
    [CanBeNull]
    public DateTime Started { get; set; }
  }
}
