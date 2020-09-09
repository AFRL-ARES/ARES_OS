using System;
using ARESCore.DeviceSupport;

namespace ARESDevicesPlugin.Data
{
  public class FcScriptData : IDeviceScriptData
  {
    public string ExpScript { get; set; }
    public string InterExpScript { get; set; }
    public string CampaignCloseScript { get; set; }

    public FcScriptData()
    {
      ExpScript = DefaultExpScript();
      InterExpScript = DefaultInterExpScript();
      CampaignCloseScript = DefaultCampaignCloseScript();
    }

    public string DefaultExpScript()
    {
      string expScript = "// Default Sample Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";
      expScript += "// Note: This does nothing but show example of script and variable use." + "\n";

      expScript += "STEP_PLL Sample of Parallel step\n";
      expScript += "// steps are run immediately";

      expScript += "\tLASER_SHUTTER true\n\n";
      expScript += "\tWAIT 15\n\n";
      expScript += "\tLASER_POWER 0.2\n\n";
      expScript += "STEP_END\n\n";
      expScript += "// waits until all steps are complete\n";

      expScript += "STEP_SEQ steps will executed sequential\n";
      expScript += "\tLASER_SHUTTER FALSE\n\n";
      expScript += "\tWAIT 15\n\n";
      expScript += "\tLASER_POWER VAL_LASER\n\n";
      expScript += "STEP_END\n\n";

      return expScript;
    }

    public string DefaultCampaignCloseScript()
    {
      string closeScript = "// Default Close Campaign Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";

      return closeScript;
    }

    public string DefaultInterExpScript()
    {
      string interExpScript = "// Default Interexperiment Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";
      interExpScript += "STEP_SEQ Machine Cleanup: Set View Mode \n";
      interExpScript += "\tLASER_SHUTTER TRUE\n\n";

      interExpScript += "\tWAIT 15\n";
      interExpScript += "\t\n";

      interExpScript += "STEP_END\n\n";

      return interExpScript;
    }
  }
}
