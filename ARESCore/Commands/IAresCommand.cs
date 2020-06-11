using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Commands
{
  public interface IAresCommand : IBasicReactiveObjectDisposable
  {
    string ScriptName { get; }

    int ArgCount { get; }

    bool ArgCountEnforced { get; }

    string HelpString { get; }

    bool Validate( string[] args );

    bool IsPlannable { get; set; }

    string PlanValueString { get; set; }
    string CloserCmd { get; }

    Task Execute( string[] args );

  }
}