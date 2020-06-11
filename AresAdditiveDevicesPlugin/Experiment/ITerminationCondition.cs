using AresAdditiveDevicesPlugin.Experiment.Impl;

namespace AresAdditiveDevicesPlugin.Experiment
{
  public interface ITerminationCondition
  {
    TerminationConditionType TerminationType { get; set; }
  }
}
