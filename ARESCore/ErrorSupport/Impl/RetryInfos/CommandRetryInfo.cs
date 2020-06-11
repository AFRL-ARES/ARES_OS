using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Commands;
using ARESCore.Experiment.Results;

namespace ARESCore.ErrorSupport.Impl.RetryInfos
{
  public class CommandRetryInfo
  {
    public IAresCommand Command { get; set; }
    public List<string> RevisedTokens { get; set; }
    public ICommandExecutionSummary CommandExecutionSummary { get; set; }
  }
}
