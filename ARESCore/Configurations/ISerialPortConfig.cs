using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Configurations
{
  public interface ISerialPortConfig : IBasicReactiveObjectDisposable
  {
    int MaxWaitTries { get; set; }
    int DataBits { get; set; }
    int BaudRate { get; set; }
    Parity Parity { get; set; }
    int ReadTimeout { get; set; }
    string PortName { get; set; }
    StopBits StopBits { get; set; }
    int ReadBufferSize { get; set; }

    void SetSerialFields( string portName, int dataBits, int baudRate, Parity parity, StopBits stopBits, int readBufferSize, int maxWaits = 100, int readTimeout = 180 );


  }
}
