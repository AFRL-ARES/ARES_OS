using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using ARESCore.DisposePatternHelpers;
using Newtonsoft.Json;
using ReactiveUI;

namespace ARESCore.Configurations.impl
{
  public class SerialPortConfig : ReactiveSubscriber, ISerialPortConfig
  {
    private int _maxWaitTries = 100;
    private int _dataBits;
    private int _baudRate;
    private Parity _parity;
    private int _readTimeout;
    private string _portName;
    private StopBits _stopBits;
    private int _readBufferSize;

    public void SetSerialFields( string portName, int dataBits, int baudRate, Parity parity, StopBits stopBits, int readBufferSize, int maxWaits = 100, int readTimeout = 180 )
    {
      Parity = parity;
      PortName = portName;
      DataBits = dataBits;
      BaudRate = baudRate;
      StopBits = stopBits;
      MaxWaitTries = maxWaits;
      ReadTimeout = readTimeout;
      ReadBufferSize = readBufferSize;
    }

    public void LoadSerial(SerialPortConfig newData)
    {
      Parity = newData.Parity;
      PortName = newData.PortName;
      DataBits = newData.DataBits;
      BaudRate = newData.BaudRate;
      StopBits = newData.StopBits;
      MaxWaitTries = newData.MaxWaitTries;
      ReadTimeout = newData.ReadTimeout;
      ReadBufferSize = newData.ReadBufferSize;
    }

    public override string ToString()
    {
      return
          "\n   MaxWaitTries: " + MaxWaitTries +
          "\n       DataBits: " + DataBits +
          "\n       BaudRate: " + BaudRate +
          "\n         Parity: " + Parity +
          "\n    ReadTimeout: " + ReadTimeout +
          "\n       PortName: " + PortName +
          "\n       StopBits: " + StopBits +
          "\n ReadBufferSize: " + ReadBufferSize;
    }

    [JsonProperty]
    public int MaxWaitTries
    {
      get => _maxWaitTries;
      set => this.RaiseAndSetIfChanged(ref _maxWaitTries, value);
    }
    [JsonProperty]
    public int DataBits
    {
      get => _dataBits;
      set => this.RaiseAndSetIfChanged(ref _dataBits, value);
    }
    [JsonProperty]
    public int BaudRate
    {
      get => _baudRate;
      set => this.RaiseAndSetIfChanged(ref _baudRate, value);
    }
    [JsonProperty]
    public Parity Parity
    {
      get => _parity;
      set => this.RaiseAndSetIfChanged(ref _parity, value);
    }
    [JsonProperty]
    public int ReadTimeout
    {
      get => _readTimeout;
      set => this.RaiseAndSetIfChanged(ref _readTimeout, value);
    }
    [JsonProperty]
    public string PortName
    {
      get => _portName;
      set => this.RaiseAndSetIfChanged(ref _portName, value);
    }
    [JsonProperty]
    public StopBits StopBits
    {
      get => _stopBits;
      set => this.RaiseAndSetIfChanged(ref _stopBits, value);
    }
    [JsonProperty]
    public int ReadBufferSize
    {
      get => _readBufferSize;
      set => this.RaiseAndSetIfChanged(ref _readBufferSize, value);
    }
  }
}
