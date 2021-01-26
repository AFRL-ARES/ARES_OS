using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Experiment.Scripting
{
  public interface IScriptExecutor : IReactiveSubscriber
  {
    Task Run( string script, IPlannedExperimentInputs inputs );

    bool Validate( string script, IPlannedExperimentInputs inputs );
    bool IsComplete { get; set; }
  }
}