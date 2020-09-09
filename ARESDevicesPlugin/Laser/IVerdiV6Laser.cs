using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.DeviceSupport.Serial;

namespace AresCNTDevicesPlugin.Laser
{
   public interface IVerdiV6Laser : IRs232Device, IAresDevice
   {
      Task SetPower(double value);
      Task SetShutter(bool value);
      Task GetPower();
      Task GetShutter();
      double Power { get; }
      bool Shutter { get; }
   }
}
