using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.DeviceSupport.Usb
{
  public abstract class UsbDevice : BasicReactiveObjectDisposable, IUsbDevice
  {
    public abstract USBDeviceInfo USBDeviceInfo { get; set; }
  }
}