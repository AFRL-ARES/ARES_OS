using System.Collections.Generic;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.impl;

namespace ARESCore.Experiment
{


  public interface IPlannedExperimentBatchInputs : IReactiveSubscriber
  {
    int Count();
    bool HasInputs();
    List<IPlannedExperimentInputs> PlannedInputs { get; }

    double GetInput(string desc, int expNum);
    void SetExperimentBatchInputs(List<string> dataDesc, List<List<double>> data);
    void SetExperimentBatchInputs(IEnumerable<IEnumerable<ExperimentParameter>> experimentParameterEnums);
    void SetExperimentBatchInputs(IEnumerable<IPlannedExperimentInputs> plannedExperimentInputsEnum);

    void LoadInputsFromFile(string fileName, char delim = ',');
    IPlannedExperimentInputs GetExperimentInputs(int expNum);

  }
}