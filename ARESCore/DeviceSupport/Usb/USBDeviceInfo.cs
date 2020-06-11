using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARESCore.DeviceSupport.Usb
{
  public class USBDeviceInfo
  {
    public string Availability { get; set; }
    public string Caption { get; set; }
    public string ClassCode { get; set; }
    public uint ConfigManagerErrorCode { get; set; }
    public bool ConfigManagerUserConfig { get; set; }
    public string CreationClassName { get; set; }
    public string CurrentAlternateSettings { get; set; }
    public string CurrentConfigValue { get; set; }
    public string Description { get; set; }
    public string DeviceID { get; set; }
    public string ErrorCleared { get; set; }
    public string ErrorDescription { get; set; }
    public string GangSwitched { get; set; }
    public string InstallDate { get; set; }
    public string LastErrorCode { get; set; }
    public string Name { get; set; }
    public string NumberOfConfigs { get; set; }
    public string NumberOfPorts { get; set; }
    public string PNPDeviceID { get; set; }
    public string PowerManagementCapabilities { get; set; }
    public string PowerManagementSupported { get; set; }
    public string ProtocolCode { get; set; }
    public string Status { get; set; }
    public string StatusInfo { get; set; }
    public string SubclassCode { get; set; }
    public string SystemCreationClassName { get; set; }
    public string SystemName { get; set; }
    public string USBVersion { get; set; }
  }
}
