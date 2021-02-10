using System;
using System.Linq;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using Ninject;

namespace ARESCore.DeviceSupport
{
  public abstract class  AresDeviceCommand<T> : ReactiveSubscriber, IAresDeviceCommand where T : IComparable
  {
    public abstract ConstrainedValue<T> Constraints { get; set; }
    public abstract T Value { get; set; }
    public abstract string ScriptName { get; }
    public abstract int ArgCount { get; }
    public bool ArgCountEnforced { get; } = true;
    public abstract string HelpString { get; }
    public abstract bool Validate(string[] args);

    public abstract Type AssociatedDeviceType { get; set; }
    public virtual bool IsPlannable { get; set; } = false;
    public virtual string PlanValueString { get; set; }
    public virtual Enum UnitType { get; set; } = null;
    public virtual bool IsWriteOnly { get; } = false;
    public virtual string CloserCmd { get; set; } = null;

    public abstract string Serialize();
    public abstract void Deserialize(string val);
    public abstract Task Execute(string[] lines);

    public virtual bool CheckLimits(T val)
    {
      return true;
    }

    protected virtual Task SubmitCommand()
    {
      var device = AresKernel._kernel.GetAll<IAresDevice>().FirstOrDefault(t => AssociatedDeviceType.IsAssignableFrom(t.GetType()));

      if (device == null)
      {
        return Task.FromException(new NullReferenceException("Unable to locate the Associated Device"));
      }

      try
      {
        device.IssueCommand(this);
      }
      catch (Exception e)
      {
        return Task.FromException(e);
      }

      return Task.CompletedTask;
    }
  }
}
