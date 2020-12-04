using System;
using System.IO.Ports;
using System.Threading.Tasks;
using ARESCore.Configurations;
using ARESCore.DisposePatternHelpers;
using ARESCore.Experiment;
using ARESCore.UI;
using CommonServiceLocator;
using DynamicData.Binding;
using Ninject;

namespace ARESCore.DeviceSupport.Serial
{
   //This might need refactored if issues arise.. http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
   // http://stackoverflow.com/questions/11458835/finding-information-about-all-serial-devices-connected-through-usb-in-c-sharp?rq=1


   /// <summary>
   /// RS232DataChannel
   /// </summary>
   public abstract class RS232Device : BasicReactiveObjectDisposable, IRs232Device
   {
      protected RS232Device() => AresKernel._kernel.Get<ICampaign>().WhenPropertyChanged(campaign => campaign.InitiatingEStop, false).Subscribe(estopInitiated => this.HandleEStop());

      protected SerialPort SerialPort { get; set; } = new SerialPort();

      public virtual bool Open(ISerialPortConfig config, string deviceName)
      {
         int waits = 0;
         if (SerialPort == null)
            SerialPort = new SerialPort();

         if (config == null)
            return false;

         try
         {
           if (SerialPort.IsOpen)
           {
             SerialPort.Close();
             while (SerialPort.IsOpen)
             {
               if (waits++ > config.MaxWaitTries)
                 throw new Exception();
               System.Threading.Thread.Sleep(25);
             }
           }

           SerialPort.Parity = config.Parity;
           SerialPort.PortName = config.PortName;
           SerialPort.DataBits = config.DataBits;
           SerialPort.StopBits = config.StopBits;
           SerialPort.BaudRate = config.BaudRate;
           SerialPort.ReadTimeout = config.ReadTimeout;
           SerialPort.ReadBufferSize = config.ReadBufferSize;

           while (waits < config.MaxWaitTries)
           {
             try
             {
               SerialPort.Open();
               break;
             }
             catch (UnauthorizedAccessException)
             {
               waits++;
               ServiceLocator.Current.GetInstance<IAresConsole>()
                 .WriteLine("Pass " + waits + " of " + config.MaxWaitTries + " unsuccessful when trying to open " + SerialPort.PortName + " for " + deviceName);
               System.Threading.Thread.Sleep(50);
             }
           }
         }
         catch (Exception e)
         {
           AresKernel._kernel.Get<IAresConsole>().WriteLine(deviceName + " could not open COM port: " + e.Message);
           SerialPort = null;
           return false;
         }

         return true;
      }

      public virtual bool Close(ISerialPortConfig config)
      {
         int waits = 0;
         if (SerialPort == null)
            return true;

         try
         {
            if (SerialPort.IsOpen)
            {
               SerialPort.Close();
               while (SerialPort.IsOpen)
               {
                  if (waits++ > config.MaxWaitTries)
                     throw new Exception();
                  System.Threading.Thread.Sleep(25);
               }
            }
         }
         finally
         {
            SerialPort.Dispose();
            SerialPort = null;
         }
         return true;
      }

      #region SerialPort_WriteMethods

      public void Write(IAresDeviceCommand command)
      {
         Write(command.Serialize());

      }

      public void Write(byte[] buffer)
      {
         if ((SerialPort == null) || !(SerialPort.IsOpen)) return;
         SerialPort.DiscardOutBuffer();
         SerialPort.DiscardInBuffer();
         SerialPort.Write(buffer, 0, buffer.Length);
         SerialPort.BaseStream.Flush();

      }

      public void Write(string dataString)
      {
         if (SerialPort == null || !SerialPort.IsOpen) return;
         try
         {
           SerialPort.DiscardOutBuffer();
           SerialPort.DiscardInBuffer();
           SerialPort.Write(dataString);
           SerialPort.BaseStream.Flush();
         }
         catch (Exception)
         {
           // maybe we're shutting down or we changed serial ports
         }

      }

      #endregion SerialPort_WriteMethods

      #region SerialPort_ReadMethods

      public string ReadLine(int timeout = 500)
      {
         if ((SerialPort == null) || !(SerialPort.IsOpen)) return "";

         SerialPort.ReadTimeout = timeout;
         return SerialPort.ReadLine();
      }

      public string ReadExisting(int waitTime = 100)
      {
         if ((SerialPort == null) || !(SerialPort.IsOpen)) return "";

         try
         {
           System.Threading.Thread.Sleep(waitTime);
         }
         catch (Exception)
         {
           // do nothing. this probably got interrupted while we were shutting down.

         }
         return SerialPort.ReadExisting();
      }

      public byte[] ReadExistingBytes(int waitTime = 100)
      {
         if ((SerialPort == null) || !(SerialPort.IsOpen)) return null;

         System.Threading.Thread.Sleep(waitTime);
         byte[] bytes = new byte[SerialPort.BytesToRead];
         for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)SerialPort.ReadByte();
         return bytes;
      }

      public byte ReadByte(int waitTime = 100)
      {
        if ((SerialPort == null) || !(SerialPort.IsOpen)) return 0;

        System.Threading.Thread.Sleep(waitTime);
        return (byte)SerialPort.ReadByte();
      }

    public char[] Read(ISerialPortConfig config, int count)
      {
         if ((SerialPort == null) || !(SerialPort.IsOpen)) return null;

         int waits = 0;
         int readbytes = 0;
         char[] charBuffer = new char[count];

         while ((SerialPort.BytesToRead < count) && (waits < config.MaxWaitTries))
         {
            System.Threading.Thread.Sleep(10);
            waits++;
         }

         if (SerialPort.BytesToRead == count)
         {
            readbytes = SerialPort.Read(charBuffer, 0, count);
            if (readbytes != count)
               charBuffer = null;
         }

         return charBuffer;
      }

      #endregion SerialPort_ReadMethods

      #region MODBUS 

      // Functions from http://www.codeproject.com/Articles/20929/Simple-Modbus-Protocol-in-C-NET
      // Author user name: distantcity

      // Function 16 - Write Multiple Registers
      public void SendFc16(byte address, byte funccode, ushort start, ushort registers, short[] values)
      {
         //Clear in/out buffers:
         SerialPort.DiscardOutBuffer();
         SerialPort.DiscardInBuffer();

         //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
         byte[] message = new byte[9 + 2 * registers];

         //Function 16 response is fixed at 8 bytes
         byte[] response = new byte[8];

         //Add bytecount to message:
         message[6] = (byte)(registers * 2);

         //Put write values into message prior to sending:
         for (int i = 0; i < registers; i++)
         {
            message[7 + 2 * i] = (byte)(values[i] >> 8);
            message[8 + 2 * i] = (byte)(values[i]);
         }

         //Build outgoing message:
         BuildMessage(address, funccode, start, registers, ref message);

         AresKernel._kernel.Get<IAresConsole>().WriteLine("Message: A[" + address.ToString("X") + "] F[" + funccode.ToString("X") + "] S[" + start.ToString("X") + "] R[" + registers.ToString("X") + "] B[" + message[6].ToString("X") + "]");
         foreach (var value in values)
            AresKernel._kernel.Get<IAresConsole>().WriteLine("\t V[" + value.ToString("X4") + "]");

         //Send Modbus message to Serial Port:
         SerialPort.Write(message, 0, message.Length);
      }

      public void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
      {
         byte[] CRC = new byte[2];

         message[0] = address;
         message[1] = type;
         message[2] = (byte)(start >> 8);
         message[3] = (byte)start;
         message[4] = (byte)(registers >> 8);
         message[5] = (byte)registers;

         GetCRC(message, ref CRC);
         message[message.Length - 2] = CRC[0];
         message[message.Length - 1] = CRC[1];

         AresKernel._kernel.Get<IAresConsole>().WriteLine("CRC: 1[" + CRC[0].ToString("X") + "] 2[" + CRC[1].ToString("X") + "]");
      }

      public void GetResponse(ref byte[] response, int waitTime = 100)
      {
         System.Threading.Thread.Sleep(waitTime);
         for (int i = 0; i < response.Length; i++)
            response[i] = (byte)(SerialPort.ReadByte());
      }

      public bool CheckResponse(byte[] response)
      {
         byte[] CRC = new byte[2];
         GetCRC(response, ref CRC);
         return CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1];
      }

      public void GetCRC(byte[] message, ref byte[] CRC)
      {
         char CRCLSB;
         ushort CRCFull = 0xFFFF;
         byte CRCHigh = 0xFF, CRCLow = 0xFF;
         for (int i = 0; i < (message.Length) - 2; i++)
         {
            CRCFull = (ushort)(CRCFull ^ message[i]);

            for (int j = 0; j < 8; j++)
            {
               CRCLSB = (char)(CRCFull & 0x0001);
               CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

               if (CRCLSB == 1)
                  CRCFull = (ushort)(CRCFull ^ 0xA001);
            }
         }
         CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
         CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
      }

      // Function 3 - Read Registers

      #endregion MODBUS

      protected override Task HandleEStop()
      {
         // Don't trigger EStop notification
         return Task.CompletedTask;
      }
   }
}