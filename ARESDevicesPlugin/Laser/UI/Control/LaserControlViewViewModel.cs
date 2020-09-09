using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AresCNTDevicesPlugin.Laser.UI.Control
{
   public class LaserControlViewViewModel : BasicReactiveObjectDisposable
   {
      private IVerdiV6Laser _laser;
    private double _selectedPowerPreset;
    private double _reqPower;

      public LaserControlViewViewModel(IVerdiV6Laser laser)
      {
         Laser = laser;
      SelectedPowerPreset = Laser.Power;
      RequestedPower = Laser.Power;
      
         ToggleShutterCommand = ReactiveCommand.Create(() => Laser.SetShutter(!Laser.Shutter));
      }

    public double SelectedPowerPreset
    {
      get => _selectedPowerPreset;
      set
      {
        if (value != Laser.Power)
        {
          Laser.SetPower(value);
          _reqPower = Laser.Power;
        }
        this.RaiseAndSetIfChanged(ref _selectedPowerPreset, value);
      }
    }

    public double RequestedPower
      {
         get => _reqPower;
         set
         {
            if (value != Laser.Power)
            {
               Laser.SetPower(value);
               _selectedPowerPreset = Laser.Power;
            }
            this.RaiseAndSetIfChanged(ref _reqPower, value);
         }
      }

    public ObservableCollection<double> PowerPresets { get; set; } = new ObservableCollection<double>
      { 0.01, 0.05, 0.10, 0.20, 0.50, 0.60, 0.70, 0.80, 0.90, 1.00, 1.10, 1.20, 1.30, 1.40, 1.50, 1.60, 1.70, 1.80, 1.90, 2.00 };


    public IVerdiV6Laser Laser
      {
         get => _laser;
         set => this.RaiseAndSetIfChanged(ref _laser, value);
      }

      public ReactiveCommand<Unit, Task> ToggleShutterCommand { get; }
   }
}