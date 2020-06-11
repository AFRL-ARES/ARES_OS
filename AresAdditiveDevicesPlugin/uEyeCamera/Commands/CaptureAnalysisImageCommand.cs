using System;
using System.Drawing;
using System.Threading.Tasks;
using ARESCore.DataHub;
using ARESCore.DataHub.impl;
using ARESCore.DeviceSupport;
using CommonServiceLocator;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Commands
{
  public class CaptureAnalysisImageCommand : AresDeviceCommand<bool>
  {
    public override ConstrainedValue<bool> Constraints { get; set; }
    public override bool Value { get; set; }
    public override string ScriptName { get; } = "CAPTURE_ANALYSIS_IMAGE";
    public override int ArgCount { get; }

    public override string HelpString { get; } =
      "Captures and sends a bitmap taken from the first assigned analysis image over the datahub";
    public override bool Validate(string[] args)
    {
      return args.Length == 0;
    }

    public override string Serialize()
    {
      throw new NotImplementedException();
    }

    public override void Deserialize(string val)
    {
      throw new NotImplementedException();
    }

    public override async Task Execute(string[] lines)
    {
      var camRepo = ServiceLocator.Current.GetInstance<IUEyeCameraRepo>();
      var hub = ServiceLocator.Current.GetInstance<IDataHub>();
      var analysisImage = await camRepo.CaptureAnalysisImage();
      var dataEntry = new DataHubEntry(typeof(Bitmap), analysisImage);
      hub.Data = dataEntry;
    }

    public override Type AssociatedDeviceType { get; set; }
  }
}
