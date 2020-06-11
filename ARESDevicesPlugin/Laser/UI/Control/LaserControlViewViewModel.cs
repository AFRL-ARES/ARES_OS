using System;
using System.Collections.ObjectModel;
using ARESCore.DisposePatternHelpers;
using DynamicData.Binding;
using ReactiveUI;

namespace ARESDevicesPlugin.Laser.UI.Control
{
  public class LaserControlViewViewModel : BasicReactiveObjectDisposable
  {
    private IVerdiV6Laser _laser;
    private VerdiV6LaserState _targetState;
    private VerdiV6LaserState _currentState;
    private double _selectedPowerPreset;

    public LaserControlViewViewModel(IVerdiV6Laser laser)
    {
      Laser = laser;
      TargetState = new VerdiV6LaserState();
      TargetState.WhenAnyPropertyChanged().Subscribe(t => PushTargetState());
      CurrentState = (VerdiV6LaserState)Laser.CurrentState;
      SelectedPowerPreset = 0.01;
      Laser.CurrentState.WhenAnyPropertyChanged().Subscribe(c => CurrentState = (VerdiV6LaserState)c);

    }

    public double SelectedPowerPreset
    {
      get { return _selectedPowerPreset; }
      set
      {
        this.RaiseAndSetIfChanged(ref _selectedPowerPreset, value);
        TargetState.LaserPower = value;
      }
    }

    private void PushTargetState()
    {
      Laser.TargetState = TargetState;
    }

    public VerdiV6LaserState TargetState
    {
      get => _targetState;
      set
      {
        this.RaiseAndSetIfChanged(ref _targetState, value);
      }
    }

    public VerdiV6LaserState CurrentState
    {
      get => _currentState;
      set
      {
        this.RaiseAndSetIfChanged(ref _currentState, value);
      }
    }


    public ObservableCollection<double> PowerPresets { get; set; } = new ObservableCollection<double>
    { 0.01, 0.05, 0.10, 0.20, 0.50, 0.60, 0.70, 0.80, 0.90, 1.00, 1.10, 1.20, 1.30, 1.40, 1.50, 1.60, 1.70, 1.80, 1.90, 2.00 };


    public IVerdiV6Laser Laser
    {
      get { return _laser; }
      set { this.RaiseAndSetIfChanged(ref _laser, value); }
    }
  }
}