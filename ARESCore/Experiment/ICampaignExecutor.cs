﻿using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Experiment
{
  public interface ICampaignExecutor : IBasicReactiveObjectDisposable
  {
    Task Execute();
    bool IsComplete { get; set; }
    bool ShouldExecute { get; set; }
  }
}