using System;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.PythonStageController.Impl
{
  public class Location : BasicReactiveObjectDisposable, IComparable
  {
    private double _x;
    private double _y;
    private double _z;
    private double _e;
    private bool _compareZ = false;
    private bool _compareE = false;
    private double _comparisonPrecision = .01;

    public static bool TryParse(string[] parameterStrings, out Location location)
    {
      if (
        double.TryParse(parameterStrings[0].Replace(",", ""), out var x) &&
        double.TryParse(parameterStrings[1].Replace(",", ""), out var y))
      {
        location = new Location
        {
          X = x,
          Y = y
        };
        if (parameterStrings[2] != null)
        {
          if (parameterStrings[3] != null)
          {
            if (!double.TryParse(parameterStrings[3].Replace(",", ""), out var e))
            {
              return false;
            }
            location.E = e;
          }
          if (!double.TryParse(parameterStrings[2].Replace(",", ""), out var z))
          {
            return false;
          }
          location.Z = z;
        }
      }
      location = null;
      return false;
    }
    public static bool TryParse(string parametersLine, out Location location)
    {
      parametersLine = parametersLine.Replace("(", "");
      parametersLine = parametersLine.Replace(")", "");
      parametersLine = parametersLine.Replace("[", "");
      parametersLine = parametersLine.Replace("]", "");
      parametersLine = parametersLine.Replace("{", "");
      parametersLine = parametersLine.Replace("}", "");
      parametersLine = parametersLine.Replace("<", "");
      parametersLine = parametersLine.Replace(">", "");
      if (parametersLine.Contains(","))
      {
        return TryParse(parametersLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), out location);
      }
      return TryParse(parametersLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), out location);
    }

    public static Location Parse(string[] parameterStrings)
    {
      var x = double.Parse(parameterStrings[0].Replace(",", ""));
      var y = double.Parse(parameterStrings[1].Replace(",", ""));

      var location = new Location
      {
        X = x,
        Y = y
      };
      if (parameterStrings[2] != null)
      {
        var z = double.Parse(parameterStrings[2].Replace(",", ""));
        location.Z = z;
        if (parameterStrings[3] != null)
        {
          var e = double.Parse(parameterStrings[3].Replace(",", ""));
          location.E = e;
        }
      }
      return location;
    }

    public static Location Parse(string parametersLine)
    {
      parametersLine = parametersLine.Replace("(", "");
      parametersLine = parametersLine.Replace(")", "");
      parametersLine = parametersLine.Replace("[", "");
      parametersLine = parametersLine.Replace("]", "");
      parametersLine = parametersLine.Replace("{", "");
      parametersLine = parametersLine.Replace("}", "");
      parametersLine = parametersLine.Replace("<", "");
      parametersLine = parametersLine.Replace(">", "");
      if (parametersLine.Contains(","))
      {
        return Parse(parametersLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
      }
      return Parse(parametersLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    public int CompareTo(object obj)
    {
      var otherLocation = (Location)obj;
      // Couldn't think of a quick way to break this down into functions, but its a basic comparison that prioritizes Y --> X --> optional Z --> optional E
      // ie. The north east most location is the greatest value
      if (Math.Abs(Y - otherLocation.Y) < ComparisonPecision && Math.Abs(X - otherLocation.X) < ComparisonPecision)
      {
        if (!CompareZ)
        {
          if (!CompareE)
          {
            return 0;
          }
          if (Math.Abs(E - otherLocation.E) < ComparisonPecision)
          {
            return 0;
          }
          else if (E < otherLocation.E)
          {
            return -1;
          }
          else
          {
            return 1;
          }
        }
        if (!CompareE)
        {
          if (!CompareZ)
          {
            return 0;
          }
          if (Math.Abs(Z - otherLocation.Z) < ComparisonPecision)
          {
            return 0;
          }
          else if (Z < otherLocation.Z)
          {
            return -1;
          }
          else
          {
            return 1;
          }
        }
      }

      if (Y < otherLocation.Y)
      {
        return 1;
      }
      else if (Y > otherLocation.Y)
      {
        return -1;
      }
      else
      {
        if (X < otherLocation.X)
        {
          return -1;
        }
        if (X > otherLocation.X)
        {
          return 1;
        }
        if (CompareZ && !CompareE)
        {
          if (Z > otherLocation.Z)
          {
            return 1;
          }
          else if (Z < otherLocation.Z)
          {
            return -1;
          }
          return 0;
        }
        if (CompareE && !CompareZ)
        {
          if (E > otherLocation.E)
          {
            return 1;
          }
          if (E < otherLocation.E)
          {
            return -1;
          }
          return 0;
        }
        // Else is handled for all things equal at the start
      }
      return 0;
    }

    public static Location operator -(Location leftHandLocation, Location rightHandLocation)
    {
      var location = new Location
      {
        X = leftHandLocation.X - rightHandLocation.X,
        Y = leftHandLocation.Y - rightHandLocation.Y,
        Z = leftHandLocation.Z,
        E = leftHandLocation.E
      };
      if (leftHandLocation.CompareZ)
      {
        location.Z -= rightHandLocation.Z;
      }
      if (leftHandLocation.CompareE)
      {
        location.E -= rightHandLocation.E;
      }
      return location;
    }
    public static Location operator +(Location leftHandLocation, Location rightHandLocation)
    {
      var location = new Location
      {
        X = leftHandLocation.X + rightHandLocation.X,
        Y = leftHandLocation.Y + rightHandLocation.Y,
        Z = leftHandLocation.Z,
        E = leftHandLocation.E
      };
      if (leftHandLocation.CompareZ)
      {
        location.Z += rightHandLocation.Z;
      }
      if (leftHandLocation.CompareE)
      {
        location.E += rightHandLocation.E;
      }
      return location;
    }


    public double X
    {
      get => _x;
      set => this.RaiseAndSetIfChanged(ref _x, value);
    }

    public double Y
    {
      get => _y;
      set => this.RaiseAndSetIfChanged(ref _y, value);
    }

    public double Z
    {
      get => _z;
      set => this.RaiseAndSetIfChanged(ref _z, value);
    }

    public double E
    {
      get => _e;
      set => this.RaiseAndSetIfChanged(ref _e, value);
    }

    public bool CompareZ
    {
      get => _compareZ;
      set => this.RaiseAndSetIfChanged(ref _compareZ, value);
    }

    public bool CompareE
    {
      get => _compareE;
      set => this.RaiseAndSetIfChanged(ref _compareE, value);
    }

    public double ComparisonPecision
    {
      get => _comparisonPrecision;
      set => this.RaiseAndSetIfChanged(ref _comparisonPrecision, value);
    }
  }
}