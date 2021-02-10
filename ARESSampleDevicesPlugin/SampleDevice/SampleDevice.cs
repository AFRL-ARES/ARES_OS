using ARESCore.DeviceSupport;
using ARESCore.DisposePatternHelpers;
using ARESCore.Registries;
using ARESCore.UI;
using AresSampleDevicesPlugin.SampleDevice.Commands;
using CommonServiceLocator;
using MahApps.Metro.IconPacks;
using Prism.Ioc;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AresSampleDevicesPlugin.SampleDevice
{
  // ReSharper disable once UnusedMember.Global
  public class SampleDevice : ReactiveSubscriber, ISampleDevice
  {
    private int _readRate;
    private double _doubleValue = 0.01;
    private bool _boolVal;
    private readonly SampleDeviceDoubleConstrainedValue _valueLimits = new SampleDeviceDoubleConstrainedValue();
    private bool _connected;


    public void Init()
    {
      ServiceLocator.Current.GetInstance<ILoadingStatus>().StatusInfo = "Initializing Device";
      ServiceLocator.Current.GetInstance<ILoadingStatus>().Icon = PackIconMaterialKind.AlarmLight;
    }

    public void Activate()
    {
      ServiceLocator.Current.GetInstance<ILoadingStatus>().StatusInfo = "Activating Device";
      ServiceLocator.Current.GetInstance<ILoadingStatus>().Icon = PackIconMaterialKind.AlarmLight;
      Validate();
      if (Connected)
      {
        RequestState();
        Observable.Interval(TimeSpan.FromMilliseconds(5000)).SubscribeOn(RxApp.MainThreadScheduler).Subscribe(_ => RequestState());
      }
    }

    private void RequestState()
    {
      try
      {
        GetBool();
        GetDouble();
      }
      catch (Exception e)
      {
        ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine("Device RequestState " + e.Message);
      }
    }

    public string Name { get; set; } = "Device";
    public IAresDeviceState CurrentState { get; set; }
    public IAresDeviceState TargetState { get; set; }

    public Task GetDouble()
    {
      return Task.FromResult(DoubleValue);
    }

    public Task GetBool()
    {
      return Task.FromResult(BoolValue);

    }

    public Task SetDouble(double value)
    {
      if (value < _valueLimits.MinValue || value > _valueLimits.MaxValue)
      {
        ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine("Tried setting an invalid sample double value. Check your constraints");
        return Task.CompletedTask;
      }

      DoubleValue = value;
      Task.WaitAll(GetDouble());
      return Task.CompletedTask;
    }

    public Task SetBool(bool value)
    {
      BoolValue = value;
      return Task.CompletedTask;
    }

    public void IssueCommand(IAresDeviceCommand command)
    {
      switch (command)
      {
        case SampleDeviceSetBoolComand boolCommand:
          SetBool(boolCommand.Value);
          break;
        case SampleDeviceSetDoubleCommand valueCommand:
          SetDouble(valueCommand.Value);
          break;
      }
    }

    public void RegisterCommands(IContainerRegistry registry)
    {
      var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
      repo.Add(new SampleDeviceGetDoubleCommand());
      repo.Add(new SampleDeviceGetBoolValCommand());
      repo.Add(new SampleDeviceSetDoubleCommand());
      repo.Add(new SampleDeviceSetBoolComand());
    }

    public string GetSampleScriptEntry()
    {
      var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
      var powCommand = (IAresDeviceCommand)repo.FirstOrDefault(c => c is SampleDeviceSetDoubleCommand);
      var returnme = "\t// Set double to sample value \n\t" + powCommand.ScriptName + " 0.2\n\t\n";
      return returnme;
    }

    public int ReadRate
    {
      get => _readRate;
      set => this.RaiseAndSetIfChanged(ref _readRate, value);
    }


    public bool Connected
    {
      get => _connected;
      private set => this.RaiseAndSetIfChanged(ref _connected, value);
    }
    public double DoubleValue
    {
      get => _doubleValue;
      private set => this.RaiseAndSetIfChanged(ref _doubleValue, value);
    }

    public bool BoolValue
    {
      get => _boolVal;
      private set => this.RaiseAndSetIfChanged(ref _boolVal, value);
    }

    public string Validate()
    {
      Connected = true;
      if (Connected)
        return "";
      return "Verdi V6 Device is not Connected";
    }

    bool IAresDevice.Connected => _connected;

    protected override Task HandleEStop()
    {
      return HandleStop();
    }

    protected override Task HandleStop()
    {
      SetDouble(0.0);
      SetBool(false);
      return Task.CompletedTask;
    }
  }
}