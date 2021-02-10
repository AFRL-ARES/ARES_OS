using System.Management;
using ARESCore.DisposePatternHelpers;
using DynamicData.Binding;
using ReactiveUI;

namespace ARESCore.DeviceSupport.Usb
{
  public class UsbDeviceViewModel : ReactiveSubscriber
  {
    private ObservableCollectionExtended<USBDeviceInfo> _devices = new ObservableCollectionExtended<USBDeviceInfo>();

    public UsbDeviceViewModel()
    {
      GetUSBDevices();
    }
    private void GetUSBDevices()
    {
      var searcher = new ManagementObjectSearcher( @"Select * From Win32_USBHub" );
      var collection = searcher.Get();
      foreach ( var device in collection )
      {
        var deviceInfo = new USBDeviceInfo();
        deviceInfo.Availability = (string)device.GetPropertyValue( "Availability" );
        deviceInfo.Caption = (string)device.GetPropertyValue( "Caption" );
        deviceInfo.ClassCode = (string)device.GetPropertyValue( "ClassCode" );
        deviceInfo.ConfigManagerErrorCode = (uint)device.GetPropertyValue( "ConfigManagerErrorCode" );
        deviceInfo.ConfigManagerUserConfig = (bool)device.GetPropertyValue( "ConfigManagerUserConfig" );
        deviceInfo.CreationClassName = (string)device.GetPropertyValue( "CreationClassName" );
        deviceInfo.CurrentAlternateSettings = (string)device.GetPropertyValue( "CurrentAlternateSettings" );
        deviceInfo.CurrentConfigValue = (string)device.GetPropertyValue( "CurrentConfigValue" );
        deviceInfo.Description = (string)device.GetPropertyValue( "Description" );
        deviceInfo.DeviceID = (string)device.GetPropertyValue( "DeviceID" );
        deviceInfo.ErrorCleared = (string)device.GetPropertyValue( "ErrorCleared" );
        deviceInfo.ErrorDescription = (string)device.GetPropertyValue( "ErrorDescription" );
        deviceInfo.GangSwitched = (string)device.GetPropertyValue( "GangSwitched" );
        deviceInfo.InstallDate = (string)device.GetPropertyValue( "InstallDate" );
        deviceInfo.LastErrorCode = (string)device.GetPropertyValue( "LastErrorCode" );
        deviceInfo.Name = (string)device.GetPropertyValue( "Name" );
        deviceInfo.NumberOfConfigs = (string)device.GetPropertyValue( "NumberOfConfigs" );
        deviceInfo.NumberOfPorts = (string)device.GetPropertyValue( "NumberOfPorts" );
        deviceInfo.PNPDeviceID = (string)device.GetPropertyValue( "PNPDeviceID" );
        deviceInfo.PowerManagementCapabilities = (string)device.GetPropertyValue( "PowerManagementCapabilities" );
        deviceInfo.PowerManagementSupported = (string)device.GetPropertyValue( "PowerManagementSupported" );
        deviceInfo.ProtocolCode = (string)device.GetPropertyValue( "ProtocolCode" );
        deviceInfo.Status = (string)device.GetPropertyValue( "Status" );
        deviceInfo.StatusInfo = (string)device.GetPropertyValue( "StatusInfo" );
        deviceInfo.SubclassCode = (string)device.GetPropertyValue( "SubclassCode" );
        deviceInfo.SystemCreationClassName = (string)device.GetPropertyValue( "SystemCreationClassName" );
        deviceInfo.SystemName = (string)device.GetPropertyValue( "SystemName" );
        deviceInfo.USBVersion = (string)device.GetPropertyValue( "USBVersion" );
        Devices.Add( deviceInfo );
      }

      collection.Dispose();
      searcher.Dispose();
    }

    public ObservableCollectionExtended<USBDeviceInfo> Devices
    {
      get => _devices;
      set => this.RaiseAndSetIfChanged( ref _devices, value );
    }
  }
}
