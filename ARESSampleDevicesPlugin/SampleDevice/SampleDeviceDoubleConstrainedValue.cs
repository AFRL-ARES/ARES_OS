using ARESCore.DeviceSupport;

namespace AresSampleDevicesPlugin.SampleDevice
{
  public class SampleDeviceDoubleConstrainedValue : ConstrainedValue<double>
  {
    public SampleDeviceDoubleConstrainedValue()
    {
      MaxValue = 6.0;
      MinValue = 0.01;
    }
  }
}
