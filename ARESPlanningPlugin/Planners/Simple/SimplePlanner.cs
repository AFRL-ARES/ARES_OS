using System.Collections.Generic;
using System.Threading.Tasks;
using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using ARESCore.PlanningSupport.Impl;

namespace AresPlanningPlugin.Planners
{
  public class SimplePlanner : AresPlanner
  {

    public SimplePlanner()
    {
      CanPlan = true;
    }

    public override Task<IPlannedExperimentBatchInputs> DoPlanning()
    {
      return Task.Run(async () =>
      {

        var inputs = new PlannedExperimentBatchInputs();
        var varNames = new List<string>();
        varNames.Add("VAL_LASER");
        var varVals = new List<double>();
        varVals.Add(6);
        inputs.PlannedInputs.Add(new PlannedExperimentInputs(varNames, varVals));
        return (IPlannedExperimentBatchInputs)inputs;
      });
    }

    }

  }

