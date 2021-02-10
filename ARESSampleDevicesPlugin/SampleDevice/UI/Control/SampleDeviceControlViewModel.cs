using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ARESCore.DataHub;
using ARESCore.DataHub.impl;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment.Results;
using ARESCore.Experiment.Results.impl;
using CommonServiceLocator;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AresSampleDevicesPlugin.SampleDevice.UI.Control
{
  public class SampleDeviceControlViewModel : ReactiveSubscriber
  {
    private ISampleDevice _device;
    private double _selectedValuePreset;
    private double _reqValue;

    public SampleDeviceControlViewModel(ISampleDevice device)
    {
      Device = device;
      SelectedValuePreset = Device.DoubleValue;
      RequestedValue = Device.DoubleValue;

      ToggleBoolCommand = ReactiveCommand.Create(ToggleBool);
    }

    private Task ToggleBool()
    {
      //TODO: Deleteme, just testing stuff
      var campaignExecutionStuff = ServiceLocator.Current.GetInstance<ICampaignExecutionSummary>();
      campaignExecutionStuff.ExperimentExecutionSummaries.Add(new ExperimentExecutionSummary {ExperimentNumber = campaignExecutionStuff.ExperimentExecutionSummaries.Count});
      var testEntry = new DataHubEntry();
      ServiceLocator.Current.GetInstance<IDataHub>()
                    .Data = testEntry;



      return Device.SetBool(!Device.BoolValue);
    }

    public double SelectedValuePreset
    {
      get => _selectedValuePreset;
      set
      {
        if (value != Device.DoubleValue)
        {
          Device.SetDouble(value);
          _reqValue = Device.DoubleValue;
        }
        this.RaiseAndSetIfChanged(ref _selectedValuePreset, value);
      }
    }

    public double RequestedValue
    {
      get => _reqValue;
      set => this.RaiseAndSetIfChanged(ref _reqValue, value);
    }

    public ObservableCollection<double> ValuePresets { get; set; } = new ObservableCollection<double>
      { 0.01, 0.05, 0.10, 0.20, 0.50, 0.60, 0.70, 0.80, 0.90, 1.00, 1.10, 1.20, 1.30, 1.40, 1.50, 1.60, 1.70, 1.80, 1.90, 2.00 };


    public ISampleDevice Device
    {
      get => _device;
      set => this.RaiseAndSetIfChanged(ref _device, value);
    }

    public ReactiveCommand<Unit, Task> ToggleBoolCommand { get; }

    public void CommitDoubleValue()
    {
      if (_reqValue != Device.DoubleValue)
      {
        Device.SetDouble(_reqValue);
        _selectedValuePreset = Device.DoubleValue;
      }
    }
  }
}