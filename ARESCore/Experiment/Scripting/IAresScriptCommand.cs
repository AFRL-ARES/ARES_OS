using ARESCore.Commands;

namespace ARESCore.Experiment.Scripting
{
  public interface IAresScriptCommand : IAresCommand
  {
    bool IsComplete { get; set; }
  }
}
