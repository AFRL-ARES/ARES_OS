using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.Experiment.Scripting
{
  public abstract class AresScriptCommand<T> : ReactiveSubscriber, IAresScriptCommand
  {
    private bool _isComplete;

    public abstract string ScriptName { get; }
    public abstract int ArgCount { get; }
    public virtual bool ArgCountEnforced { get; } = true;
    public abstract string HelpString { get; }
    public abstract bool Validate( string[] args );
    public bool IsPlannable { get; set; } = false;
    public string PlanValueString { get; set; }
    public virtual string CloserCmd { get; }

    public abstract Task Execute( string[] lines );
    public virtual ObservableCollection<T> ArgList { get; set; } = new ObservableCollection<T>();

    public bool IsComplete
    {
      get => _isComplete;
      set
      {
        _isComplete = value;
        this.RaisePropertyChanged();
      }
    }
  }
}
