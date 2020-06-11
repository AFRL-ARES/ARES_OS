using System.Collections.Generic;

namespace AresAdditiveDevicesPlugin.UEyeCamera
{
  public class CameraConfig
  {
    // TODO: use Model/Serial/USB Port/Something to identify what each camera is used for (assign CameraType)
    public List<string> AnalysisCamerasSerialNumbers { get; set; }
    public List<string> ProcessCamerasSerialNumbers { get; set; }
  }
}
