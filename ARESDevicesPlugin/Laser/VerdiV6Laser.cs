using System;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AresCNTDevicesPlugin.Laser.Commands;
using AresCNTDevicesPlugin.Laser.Config;
using ARESCore.DeviceSupport;
using ARESCore.DeviceSupport.Serial;
using ARESCore.Registries;
using ARESCore.UI;
using CommonServiceLocator;
using MahApps.Metro.IconPacks;
using Prism.Ioc;
using ReactiveUI;

namespace AresCNTDevicesPlugin.Laser
{
   public class VerdiV6Laser : RS232Device, IVerdiV6Laser
   {
      private int _readRate;
      private double _laserPower = 0.01;
      private bool _laserShutter = false;
      private LaserPowerConstrainedValue _powerLimits = new LaserPowerConstrainedValue();
      private bool _connected = false;

      public VerdiV6Laser()
      {
      }

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
         Validate();
#endif
         if (Connected)
         {
            RequestState();
            Observable.Interval(TimeSpan.FromMilliseconds(5000)).SubscribeOn(RxApp.MainThreadScheduler).Subscribe(_ => RequestState());
         }
      }

      private void RequestState()
      {
         if (SerialPort == null || !SerialPort.IsOpen) return;

         try
         {
            GetShutter();
            GetPower();
         }
         catch (Exception e)
         {
            ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine("Laser RequestState " + e.Message);
         }
      }

      public string Name { get; set; } = "Laser";
      public IAresDeviceState CurrentState { get; set; }
      public IAresDeviceState TargetState { get; set; }

      public Task GetPower()
      {
#if SIMULATED
         return Task.FromResult(Power);
#endif

         Write("?SP\r\n");
         var readString = ReadExisting();
         var getterStart = "?SP";
         if ((readString.Length <= 4 + getterStart.Length) || !double.TryParse(readString.Substring(getterStart.Length, 4), out double result))
            return Task.FromResult(false);
         Power = result;
         return Task.FromResult(result);
      }

      public Task GetShutter()
      {
#if SIMULATED
         return Task.FromResult(Shutter);
#endif

         Write("?S\r\n");
         var readString = ReadExisting();
         if (string.IsNullOrEmpty(readString))
            return Task.FromResult(false);
         Shutter = readString.Contains("1");
         return Task.FromResult(Shutter);

      }

      public Task SetPower(double value)
      {
         if (value < _powerLimits.MinValue || value > _powerLimits.MaxValue)
         {
            ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine("Tried setting an invalid laser power. Quit trying to destroy things");
            return Task.CompletedTask;
         }
#if SIMULATED
         Power = value;
         return Task.CompletedTask;
#endif

         Write("P=" + value + "\r\n");
         Task.WaitAll(GetPower());
         return Task.CompletedTask;
      }

      public Task SetShutter(bool value)
      {
#if SIMULATED
         Shutter = value;
         return Task.CompletedTask;
#endif

         Write("S=" + (value ? "1" : "0") + "\r\n");
         Task.WaitAll(GetShutter());
         return Task.CompletedTask;
      }

      public void IssueCommand(IAresDeviceCommand command)
      {
         switch (command)
         {
            case LaserSetShutterCommand shutterCommand:
               SetShutter(shutterCommand.Value);
               break;
            case LaserSetPowerCommand powerCommand:
               SetPower(powerCommand.Value);
               break;
         }
      }

      public void RegisterCommands(IContainerRegistry registry)
      {
         var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
         repo.Add(new LaserGetPowerCommand());
         repo.Add(new LaserGetShutterCommand());
         repo.Add(new LaserSetPowerCommand());
         repo.Add(new LaserSetShutterCommand());
      }

      public string GetSampleScriptEntry()
      {
         var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
         IAresDeviceCommand powCommand = (IAresDeviceCommand)repo.FirstOrDefault(c => c is LaserSetPowerCommand);
         string returnme = "\t// Set laser to prescan power\n\t" + powCommand.ScriptName + " 0.2\n\t\n";
         return returnme;
      }

      public int ReadRate
      {
         get => _readRate;
         set => this.RaiseAndSetIfChanged(ref _readRate, value);
      }

      public LaserPowerConstrainedValue PowerLimits
      {
         get => _powerLimits;
         set => this.RaiseAndSetIfChanged(ref _powerLimits, value);
      }

      public bool Connected
      {
         get => _connected;
         private set => this.RaiseAndSetIfChanged(ref _connected, value);
      }
      public double Power
      {
         get => _laserPower;
         private set => this.RaiseAndSetIfChanged(ref _laserPower, value);
      }

      public bool Shutter
      {
         get => _laserShutter;
         private set => this.RaiseAndSetIfChanged(ref _laserShutter, value);
      }

      public string Validate()
      {
         Connected = SerialPort != null && SerialPort.IsOpen;
         if (Connected)
            return "";
         return "Verdi V6 Laser is not Connected";
      }

      bool IAresDevice.Connected => _connected;

      protected override Task HandleEStop()
      {
         return HandleStop();
      }

      protected override Task HandleStop()
      {
         SetPower(0.0);
         SetShutter(false);
         return Task.CompletedTask;
      }
   }
}