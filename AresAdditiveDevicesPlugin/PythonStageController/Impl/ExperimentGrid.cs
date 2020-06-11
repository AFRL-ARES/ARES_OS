using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using AresAdditiveDevicesPlugin.PythonInterop;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.PythonStageController.Impl
{
  // Each bool represents whether the experiment at the (x,y) position is available for an experiment to be executed
  public class ExperimentGrid : ObservableCollectionExtended<bool>, IExperimentGrid
  {
    private int _rows;
    private int _columns;
    private double _extentX;
    private double _extentY;
    private double _separationX;
    private double _separationY;
    private double _analysisStepX;
    private double _analysisStepY;
    private double _analysisStepZ;
    private double _extentZ;
    private double _limNorth;
    private double _limWest;
    private double _limSouth;
    private double _limEast;
    private double _initXPosition;
    private double _initYPosition;
    private double _initZPosition;
    private double _initExtruderPosition;
    private readonly IPythonBindings _bindings;
    private int _observableCount;

    public ExperimentGrid(IPythonBindings bindings)
    {
      _bindings = bindings;
    }

    public void GenerateGrid()
    {
      Clear();
      Columns = (int)Math.Floor((LimitEast - LimitWest) / (ExtentX + SeparationX));
      Rows = (int)Math.Floor((LimitNorth - LimitSouth) / (ExtentY + SeparationY));

      for (var i = 0; i < Rows * Columns; i++)
      {
        Add(true);
      }
    }

    public Point GetStartingPointIdle(int x, int y)
    {
      var xPos = LimitWest + (x * (ExtentX + SeparationX));
      var yPos = LimitSouth + ((Rows - y - 1) * (ExtentY + SeparationY));
      return new Point(xPos, yPos);
    }

    public Point GetStartingPointAbsolute(int x, int y)
    {
      var xPos = LimitWest + (x * (ExtentX + SeparationX));
      var yPos = LimitSouth + (Rows - y - 1) * (ExtentY + SeparationY);
      return new Point(xPos, yPos);
    }

    public Point GetStartingPointAbsolute(int index)
    {
      var y = index / Columns;
      var x = index % Columns;
      return GetStartingPointAbsolute(x, y);
    }

    public Point GetStartingPointIdle(int index)
    {
      var y = index / Columns;
      var x = index % Columns;
      return GetStartingPointIdle(x, y);
    }

    public Point GetStartingPointRelative(int x, int y)
    {
      if (x == 0 && y == 0)
      {
        return new Point(0, 0);
      }
      var prevX = x - 1;
      var prevY = y;
      if (prevX < 0)
      {
        prevY = y - 1;
        prevX = Columns;
      }
      var step = GetStartingPointAbsolute(x, y) - GetStartingPointAbsolute(prevX, prevY);
      return new Point(step.X, step.Y);
    }

    public Point GetStartingPointRelative(int index)
    {
      var y = index / Rows;
      var x = index % Rows;
      return GetStartingPointRelative(x, y);
    }

    public Point GetStartingPointRelative(Point currentPoint)
    {
      var index = GetIndex(currentPoint);
      return GetStartingPointRelative(index);
    }

    // 1D position to 2D travel to return 1D
    private int GetNextIndex(int startIndex)
    {
      var startX = startIndex % Columns;
      var startY = startIndex / Columns;

      // Looks immediately to the right if it can, else move up to the leftmost index in the above row if it exists, else go to the bottom left
      var nextX = startX + 1;
      var nextY = startY;

      if (nextX >= Columns)
      {
        nextX %= Columns;
        nextY--;
      }
      nextY = nextY >= 0 ? nextY : Rows - 1;
      return nextY * Columns + nextX;
    }

    public int NextAvailableIndex(Point currentPoint)
    {
      var startIndex = GetIndex(currentPoint);

      if (startIndex < 0 || this[startIndex])
      {
        return startIndex;
      }


      var travelIndex = GetNextIndex(startIndex);
      while (travelIndex != startIndex && !this[travelIndex])
      {
        travelIndex = GetNextIndex(travelIndex);
      }
      if (travelIndex == startIndex)
      {
        return -1;
      }
      return travelIndex;
    }

    public int GetIndex(Point currentPoint)
    {
      if (Rows < 1 || Columns < 1)
      {
        return -1;
      }
      if (currentPoint.X < LimitWest)
      {
        currentPoint.X = LimitWest;
      }
      if (currentPoint.X > LimitEast)
      {
        currentPoint.X = LimitEast;
      }
      if (currentPoint.Y > LimitNorth)
      {
        currentPoint.Y = LimitNorth;
      }
      if (currentPoint.Y < LimitSouth)
      {
        currentPoint.Y = LimitSouth;
      }
      if (currentPoint.X < LimitWest || currentPoint.X > LimitEast || currentPoint.Y > LimitNorth || currentPoint.Y < LimitSouth)
      {
        return -1;
      }

      var zeroX = currentPoint.X - LimitWest;
      var zeroY = currentPoint.Y - LimitSouth;

      int x, y;
      for (x = 0; (x + 1) * (ExtentX + SeparationX) <= zeroX; x++)
        ;

      for (y = 0; y < Rows - 1 && (y + 1) * (ExtentY + SeparationY) <= zeroY; y++)
        ;

      return (Rows - (y + 1)) * Columns + x; // Assume real origin is bottom left corner
    }

    public bool this[int x, int y]
    {
      get
      {
        if (x < 0 || y < 0 || x > Columns || y > Rows)
        {
          return false;
        }
        return this[(Rows - 1 - y) * Columns + x];
      }
      set => this[(Rows - 1 - y) * Columns + x] = value;
    }

    public bool this[Point location]
    {
      get
      {
        if (Rows < 1 || Columns < 1)
        {
          return false;
        }
;
        if (location.X < LimitWest || location.X > LimitEast || location.Y > LimitNorth || location.Y < LimitSouth)
        {
          return false;
        }

        var relativeY = location.Y - LimitSouth;
        var relativeX = location.X - LimitWest;

        // Determine the index we're currently at. This is not specific to the starting position of the experiment.
        var y = (int)(relativeY / (ExtentY + SeparationY));
        var x = (int)(relativeX / (ExtentX + SeparationX));
        return this[x, y];
      }

      set => this[GetIndex(location)] = value;
    }

    public double AnalysisStepX
    {
      get => _analysisStepX;
      set
      {
        if (_analysisStepX != value)
        {
          _analysisStepX = value;
          OnPropertyChanged(new PropertyChangedEventArgs("AnalysisStepX"));
        }
      }
    }

    public double AnalysisStepY
    {
      get => _analysisStepY;
      set
      {
        if (_analysisStepY != value)
        {
          _analysisStepY = value;
          OnPropertyChanged(new PropertyChangedEventArgs("AnalysisStepY"));
        }
      }
    }

    public double AnalysisStepZ
    {
      get => _analysisStepZ;
      set
      {
        if (_analysisStepZ != value)
        {
          _analysisStepZ = value;
          OnPropertyChanged(new PropertyChangedEventArgs("AnalysisStepZ"));
        }
      }
    }


    public double SeparationX
    {
      get => _separationX;
      set
      {
        if (_separationX != value)
        {
          _separationX = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("SeparationX"));
        }
      }
    }

    public double SeparationY
    {
      get => _separationY;
      set
      {
        if (_separationY != value)
        {
          _separationY = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("SeparationY"));
        }
      }
    }

    public double ExtentX
    {
      get => _extentX;
      set
      {
        if (_extentX != value)
        {
          _extentX = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("ExtentX"));
        }
      }
    }

    public double ExtentY
    {
      get => _extentY;
      set
      {
        if (_extentY != value)
        {
          _extentY = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("ExtentY"));
        }
      }
    }

    public double ExtentZ
    {
      get => _extentZ;
      set
      {
        if (_extentZ != value)
        {
          _extentZ = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("ExtentZ"));
        }
      }
    }

    public int Rows
    {
      get => _rows;
      set
      {
        if (_rows != value)
        {
          _rows = value;
          OnPropertyChanged(new PropertyChangedEventArgs("Rows"));
          ObservableCount = Rows * Columns;
        }
      }
    }

    public int Columns
    {
      get => _columns;
      set
      {
        if (_columns != value)
        {
          _columns = value;
          OnPropertyChanged(new PropertyChangedEventArgs("Columns"));
          ObservableCount = Rows * Columns;
        }
      }
    }

    public double LimitNorth
    {
      get => _limNorth;
      set
      {
        if (_limNorth != value)
        {
          _limNorth = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("LimitNorth"));
        }
      }
    }

    public double LimitEast
    {
      get => _limEast;
      set
      {
        if (_limEast != value)
        {
          _limEast = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("LimitEast"));
        }
      }
    }

    public double LimitSouth
    {
      get => _limSouth;
      set
      {
        if (_limSouth != value)
        {
          _limSouth = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("LimitSouth"));
        }
      }
    }

    public double LimitWest
    {
      get => _limWest;
      set
      {
        if (_limWest != value)
        {
          _limWest = value;
          if (this.Any())
          {
            GenerateGrid();
          }
          OnPropertyChanged(new PropertyChangedEventArgs("LimitWest"));
        }
      }
    }

    public double InitXPosition
    {
      get => _initXPosition;
      set
      {
        if (_initXPosition != value)
        {
          _initXPosition = value;
          OnPropertyChanged(new PropertyChangedEventArgs("InitXPosition"));
        }
      }
    }

    public double InitYPosition
    {
      get => _initYPosition;
      set
      {
        if (_initYPosition != value)
        {
          _initYPosition = value;
          OnPropertyChanged(new PropertyChangedEventArgs("InitYPosition"));
        }
      }
    }

    public double InitZPosition
    {
      get => _initZPosition;
      set
      {
        if (_initZPosition != value)
        {
          _initZPosition = value;
          OnPropertyChanged(new PropertyChangedEventArgs("InitZPosition"));
        }
      }
    }

    public double InitExtruderPosition
    {
      get => _initExtruderPosition;
      set
      {
        if (_initExtruderPosition != value)
        {
          _initExtruderPosition = value;
          OnPropertyChanged(new PropertyChangedEventArgs("InitExtruderPosition"));
        }
      }
    }

    public int ObservableCount
    {
      get => _observableCount;
      set
      {
        if (_observableCount != value)
        {
          _observableCount = value;
          OnPropertyChanged(new PropertyChangedEventArgs("ObservableCount"));
        }
      }
    }
  }
}
