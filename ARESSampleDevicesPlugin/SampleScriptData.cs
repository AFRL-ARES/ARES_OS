using ARESCore.DeviceSupport;
using System;

namespace AresSampleDevicesPlugin
{
  public class SampleScriptData : IDeviceScriptData
  {
    public string ExpScript { get; set; }
    public string InterExpScript { get; set; }
    public string CampaignCloseScript { get; set; }

    public SampleScriptData()
    {
      ExpScript = DefaultExpScript();
      InterExpScript = DefaultInterExpScript();
      CampaignCloseScript = DefaultCampaignCloseScript();
    }

    public string DefaultExpScript()
    {
      var expScript = "// Default Sample Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";


      expScript += "STEP_PLL Machine Setup Part 1\n";
      expScript += "\t// Set sample device double value\n";
      expScript += "\tSAMPLE_SETDOUBLE 0.2\n\n";
      expScript += "STEP_END\n\n";


      expScript += "STEP_SEQ Sample Experiment Execution\n";
      expScript += "\t// Set the sample device boolean value\n";
      expScript += "\tSAMPLE_SETDOUBLE VAL_SAMPLE\n\n";
      expScript += "\tSAMPLE_SETBOOL TRUE\n";
      expScript += "\tWAIT 10\n\n";
      expScript += "\t// Turn off the sample device boolean value\n";
      expScript += "\tSAMPLE_SETBOOL FALSE\n";
      expScript += "STEP_END\n\n";

      return expScript;
    }

    public string DefaultCampaignCloseScript()
    {
      var closeScript = "// Default Close Campaign Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";

      return closeScript;
    }

    public string DefaultInterExpScript()
    {
      var interExpScript = "// Default Interexperiment Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";
      interExpScript += "STEP_SEQ Reset fields \n";
      interExpScript += "\tSAMPLE_SETDOUBLE 0.2\n\n";
      interExpScript += "\tSAMPLE_SETBOOL FALSE\n";

      interExpScript += "\t// Let everything finish resetting\n";
      interExpScript += "\tWAIT 5\n";
      interExpScript += "\t\n";

      interExpScript += "\t// All done. Next experiment. \n";
      interExpScript += "STEP_END\n\n";

      return interExpScript;
    }
  }
}
