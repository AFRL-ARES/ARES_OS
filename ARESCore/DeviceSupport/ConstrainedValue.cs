using System;

namespace ARESCore.DeviceSupport
{
  public class ConstrainedValue<T> where T : IComparable
  {
    public T MaxValue { get; set; }

    public T MinValue { get; set; }

    public virtual T Constrain(T input)
    {
      if (input.CompareTo(MaxValue) > 0)
        return MaxValue;
      if (input.CompareTo(MinValue) < 0)
        return MinValue;
      return input;
    }
  }
}
