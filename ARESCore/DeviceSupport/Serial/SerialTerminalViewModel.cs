using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Reactive;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.DeviceSupport.Serial
{
  public class SerialTerminalViewModel : ReactiveSubscriber
  {
    private ObservableCollection<double> _stopBits = new ObservableCollection<double>() { 0, 1.0, 1.5, 2.0 };
    private ObservableCollection<int> _dataBits = new ObservableCollection<int>() { 8, 7, 6, 5 };
    private ObservableCollection<int> _baudRates = new ObservableCollection<int>() { 75, 110, 134, 150, 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 38400, 57600, 115200, 128000 };
    private ObservableCollection<string> _portNames = new ObservableCollection<string>();
    private bool _portEnabled;
    private string _textToSend;
    private double _selectedStopBits;
    private Parity _selectedParity;
    private int _selectedDataBits;
    private string _selectedComPort;
    private int _selectedBaudRate;
    private bool _settingsEnabled;
    private SerialPort _serialPort;
    private string _receivedText;
    private bool _fieldsSelected;

    public SerialTerminalViewModel()
    {
      SetPortList();
      SelectedBaudRate = 19200;
      SelectedDataBits = 8;
      SelectedParity = Parity.None;
      SelectedStopBits = 1.0;
      SettingsEnabled = true;
      FieldsSelected = false;
      PortEnabled = false;
      OpenPortCommand = ReactiveCommand.Create<Unit, Unit>( u =>
      {
        _serialPort?.Close();
        _serialPort = new SerialPort( SelectedComPort );
        _serialPort.BaudRate = SelectedBaudRate;
        _serialPort.Parity = SelectedParity;
        _serialPort.StopBits = SelectedStopBits == 1.0 ? System.IO.Ports.StopBits.One : SelectedStopBits == 1.5 ? System.IO.Ports.StopBits.OnePointFive : SelectedStopBits == 2.0 ? System.IO.Ports.StopBits.Two : System.IO.Ports.StopBits.None;
        _serialPort.DataBits = SelectedDataBits;
        _serialPort.DataReceived += ( sender, args ) => ReceivedText = _serialPort.ReadExisting();
        _serialPort.Open();
        SettingsEnabled = false;
        PortEnabled = true;
        return new Unit();
      } );

      ClosePortCommand = ReactiveCommand.Create<Unit, Unit>( u =>
      {
        _serialPort?.Close();
        SettingsEnabled = true;
        PortEnabled = false;
        return new Unit();
      } );

      SendTestCommand = ReactiveCommand.Create<Unit, Unit>( u =>
    {
      if ( _serialPort == null || !_serialPort.IsOpen )
      {
        return new Unit();
      }
      _serialPort.Write( TextToSend );
      return new Unit();
    } );
    }

    public ReactiveCommand<Unit, Unit> ClosePortCommand { get; set; }

    public string ReceivedText
    {
      get => _receivedText;
      set => this.RaiseAndSetIfChanged( ref _receivedText, value );
    }

    public ObservableCollection<double> StopBits
    {
      get => _stopBits;
      set => this.RaiseAndSetIfChanged( ref _stopBits, value );
    }

    public ObservableCollection<int> DataBits
    {
      get => _dataBits;
      set => this.RaiseAndSetIfChanged( ref _dataBits, value );
    }

    public ObservableCollection<int> BaudRates
    {
      get => _baudRates;
      set => this.RaiseAndSetIfChanged( ref _baudRates, value );
    }

    public ObservableCollection<string> PortNames
    {
      get => _portNames;
      set => this.RaiseAndSetIfChanged( ref _portNames, value );
    }

    public bool SettingsEnabled
    {
      get => _settingsEnabled;
      set => this.RaiseAndSetIfChanged( ref _settingsEnabled, value );
    }

    public int SelectedBaudRate
    {
      get => _selectedBaudRate;
      set => this.RaiseAndSetIfChanged( ref _selectedBaudRate, value );
    }

    public string SelectedComPort
    {
      get => _selectedComPort;
      set
      {
        if ( value != null && value.Length > 0 )
          FieldsSelected = true;
        this.RaiseAndSetIfChanged( ref _selectedComPort, value );
      }
    }

    public int SelectedDataBits
    {
      get => _selectedDataBits;
      set => this.RaiseAndSetIfChanged( ref _selectedDataBits, value );
    }

    public Parity SelectedParity
    {
      get => _selectedParity;
      set => this.RaiseAndSetIfChanged( ref _selectedParity, value );
    }

    public double SelectedStopBits
    {
      get => _selectedStopBits;
      set => this.RaiseAndSetIfChanged( ref _selectedStopBits, value );
    }

    public string TextToSend
    {
      get => _textToSend;
      set => this.RaiseAndSetIfChanged( ref _textToSend, value );
    }

    public bool PortEnabled
    {
      get => _portEnabled;
      set => this.RaiseAndSetIfChanged( ref _portEnabled, value );
    }

    public ReactiveCommand<Unit, Unit> OpenPortCommand { get; set; }

    public ReactiveCommand<Unit, Unit> SendTestCommand { get; set; }

    public bool FieldsSelected
    {
      get => _fieldsSelected;
      set => this.RaiseAndSetIfChanged( ref _fieldsSelected, value );
    }

    private void SetPortList()
    {
      string[] portNames = SerialPort.GetPortNames();

#if SIMULATE
      for ( int i = 0; i < portNames.Length; i++ )
      {
        SerialPort aPort = new SerialPort( portNames[i] );
        try
        {
          aPort.Open();
        }
        catch ( Exception e )
        {
          portNames[i] += " (IN USE)";
          AresKernel._kernel.Get<IAresConsole>().WriteLine( e.Message );
          continue;
        }

        if ( aPort.IsOpen )
        {
          try
          {
            aPort.Close();
          }
          catch ( Exception e )
          {
            AresKernel._kernel.Get<IAresConsole>().WriteLine( e.Message );
          }
        }
      }
#endif
      PortNames.AddRange( portNames );
    }
  }
}
