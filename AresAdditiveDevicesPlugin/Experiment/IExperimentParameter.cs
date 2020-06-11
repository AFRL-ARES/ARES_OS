using AresAdditiveDevicesPlugin.Experiment.Impl;
using ARESCore.DisposePatternHelpers;

namespace AresAdditiveDevicesPlugin.Experiment
{
  public interface IExperimentParameter : IBasicReactiveObjectDisposable
  {
    bool IsPlanned { get; set; }
    VarEntry VarEntry { get; set; }
  }
}
