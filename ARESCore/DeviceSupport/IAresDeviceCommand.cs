using System;
using ARESCore.Commands;

namespace ARESCore.DeviceSupport
{
  public interface IAresDeviceCommand: IAresCommand
  {
    Enum UnitType { get; set; }
    
    bool IsWriteOnly { get; }

    string Serialize();

    void Deserialize(string val);

    Type AssociatedDeviceType { get; set; }
  }
}
