using System.Collections.Generic;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Experiment
{


  public interface IPlannedExperimentBatchInputs : IBasicReactiveObjectDisposable
  {
    int Count();
    bool HasInputs();
    List<IPlannedExperimentInputs> PlannedInputs { get; }

    double GetInput(string desc, int expNum);
    void SetExperimentBatchInputs(List<string> dataDesc, List<List<double>> data);
    void LoadInputsFromFile(string fileName, char delim = ',');
    IPlannedExperimentInputs GetExperimentInputs(int expNum);

  }
}