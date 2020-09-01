using System;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.DeviceSupport.Serial;
using ARESCore.Registries;
using ARESCore.UI;
using ARESDevicesPlugin.Laser.Config;
using CommonServiceLocator;
using MahApps.Metro.IconPacks;
using Prism.Ioc;
using ReactiveUI;

namespace ARESDevicesPlugin.Laser
{
  public class VerdiV6Laser : RS232Device, IVerdiV6Laser
  {
    private VerdiV6LaserState _targetState = new VerdiV6LaserState();
    private VerdiV6LaserState _currentState = new VerdiV6LaserState();
    private int _readRate;

    public void Init()
    {
      ServiceLocator.Current.GetInstance<ILoadingStatus>().StatusInfo = "Initializing Laser";
      ServiceLocator.Current.GetInstance<ILoadingStatus>().Icon = PackIconMaterialKind.AlarmLight;
      if (string.IsNullOrEmpty(ServiceLocator.Current.GetInstance<ILaserAppConfig>().PortName))
        ServiceLocator.Current.GetInstance<ILaserAppConfig>().SetSerialFields("COM41", 8, 19200, Parity.None, StopBits.One, 512);
    }

    public void Activate()
    {

      ServiceLocator.Current.GetInstance<ILoadingStatus>().StatusInfo = "Activating Laser";
#if !SIMULATED
      Open(ServiceLocator.Current.GetInstance<ILaserAppConfig>());
#endif
      RequestState();
      var newTarg = new VerdiV6LaserState();
      newTarg.LaserPower = (CurrentState as VerdiV6LaserState).LaserPower;
      newTarg.LaserShutter = (CurrentState as VerdiV6LaserState).LaserShutter;
      TargetState = newTarg;
      Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(_ => RequestState());
    }

    private void RequestState()
    {
    }

    private void CreateCommand(VerdiV6LaserState laserState)
    {
    }

    public string Name { get; set; } = "Laser";

    public IAresDeviceState CurrentState
    {
      get => _currentState;
      set
      {
        var state = value as VerdiV6LaserState;
        if (state != null)
          this.RaiseAndSetIfChanged(ref _currentState, state);
      }
    }

    public IAresDeviceState TargetState
    {
      get => _targetState;
      set
      {
        var state = value as VerdiV6LaserState;
        if (state != null)
        {
          this.RaiseAndSetIfChanged(ref _targetState, state);
          CreateCommand(state);
        }
      }
    }


    private IAresDeviceCommand SubmitCommand(IAresDeviceCommand command)
    {
      bool istype = command.AssociatedDeviceType.IsAssignableFrom(GetType());
      if (!istype && command.AssociatedDeviceType != GetType())
        return null;
#if SIMULATED
      //ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine( "Laser device writing " + command.GetType().Name + ": " + command.Serialize() );
#else
      try
      {
        Write(command);
        if (!command.IsWriteOnly)
        {
          var readString = ReadLine();
          command.Deserialize(readString);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(Name + " SerialPort exception!", ex);

        //        throw new IDataSinkWriteException( Name + " SerialPort exception!", ex );
      }
#endif
      return command;
    }

    public void IssueCommand(IAresDeviceCommand command)
    {

    }

    public void RegisterCommands(IContainerRegistry registry)
    {
      var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
    }

    public string GetSampleScriptEntry()
    {
      var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
      //IAresDeviceCommand powCommand = (IAresDeviceCommand)repo.FirstOrDefault(c => c is LaserSetPowerCommand);
      string returnme = "\t// This is a test. This is only a test.\n\t\n";
      return returnme;
    }

    public int ReadRate
    {
      get => _readRate;
      set => this.RaiseAndSetIfChanged(ref _readRate, value);
    }

    public string Validate()
    {
      if (SerialPort != null && SerialPort.IsOpen)
        return "";
      return "Verdi V6 Laser is not Connected";
    }

    public bool Connected { get; }

    protected override Task HandleEStop()
    {
      return HandleStop();
    }

    protected override Task HandleStop()
    {
      ((VerdiV6LaserState)TargetState).LaserShutter = false;
      ((VerdiV6LaserState)TargetState).LaserPower = 0;
      return Task.CompletedTask;
    }
  }
}