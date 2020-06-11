using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using ARESCore.AnalysisSupport;
using ARESCore.Database.Filtering;
using ARESCore.Database.Filtering.Impl;
using ARESCore.Database.Tables;
using ARESCore.Experiment.impl;
using ARESCore.Experiment.Scripting;
using ARESCore.Experiment.Scripting.Impl;
using ARESCore.PlanningSupport;

namespace ARESCore.Experiment
{
    public interface IExperimentBatch
    {
      IPlannedExperimentBatchInputs Inputs { get; set; }
      IAresAnalyzer Analyzer { get; set; }
      IPlannedExperimentBatchInputs BatchInputs { get; set; }
      IAresPlannerManager PlannerManager { get; set; }
      ObservableCollection<ExperimentEntity> PlanningDatabase { get; set; }
      ExperimentFilterOptions PlannerExperimentFilterOptions { get; set; }
      ARESExperimentBatch.DataSaveType SaveType { get; set; }

      string RawExperimentScript { get; set; } 
      ObservableCollection<ScriptStep> RecipeSteps { get; set; }

    // [Step 1.a; called once before batch execution] Called to verify everything is in line and ready to run
    Task VerifyExperimentBatch(CancellationToken cancelToken);

        // [Step 1.b; called once before batch execution] Called to startup and inputs collection needed
        Task InitDataCollection(CancellationToken cancelToken);

        // [Step 2.a; called after each experiment] Called to do any inputs processing needed
        Task DoCurrentExperimentPostProcessing(CancellationToken cancelToken);

        // [Step 2.b; called after each experiment] Called to save the inputs the experiment produced
        Task SaveCurrentExperimentData(CancellationToken cancelToken);

        // [Step 3; called after some # of experiments] Called to do replanning and replace the IPlannedExperimentInputs
        Task DoReplanning(CancellationToken cancelToken);

        // [Step 4; called once after batch execution] Called to perform an final post processing needed on the batch
        Task DoBatchPostProcessing(CancellationToken cancelToken);
    }
}
