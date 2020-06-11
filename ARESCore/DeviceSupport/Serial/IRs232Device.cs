using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Configurations;

namespace ARESCore.DeviceSupport.Serial
{
  public interface IRs232Device
  {
    bool Open(ISerialPortConfig config);
    bool Close( ISerialPortConfig config );
    void Write( IAresDeviceCommand command );
    void Write( byte[] buffer );
    void Write( string dataString );
    string ReadLine( int timeout );
    string ReadExisting( int waitTime );
    byte[] ReadExistingBytes( int waitTime = 100 );
    char[] Read( ISerialPortConfig config, int count );
    void SendFc16( byte address, byte funccode, ushort start, ushort registers, short[] values );
    void BuildMessage( byte address, byte type, ushort start, ushort registers, ref byte[] message );
    void GetResponse( ref byte[] response, int waitTime = 100 );
    bool CheckResponse( byte[] response );
    void GetCRC( byte[] message, ref byte[] CRC );
  }
}