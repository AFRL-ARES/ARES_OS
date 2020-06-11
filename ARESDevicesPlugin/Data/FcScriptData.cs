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
      string expScript = "// Default Nanotube Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";
      expScript += "// Note: The first step of this Experiment Script assumes" + "\n";
      expScript += "//       that the microscope is currently centered on a" + "\n";
      expScript += "//       pillar that is ready for growth." + "\n";
      expScript += "// Note: Do not change the patch number between generation" + "\n";
      expScript += "//       of this batch and its running. The number of" + "\n";
      expScript += "//       available pillars is checked prior to running." + "\n\n";

      expScript += "STEP_PLL Machine Setup Part 1\n";
      expScript += "\t// Set the microscope to growth mode\n";
      expScript += "\tVIEWMODE FALSE\n\n";

      expScript += "\t// Set laser to prescan power\n";
      expScript += "\tLASER_POWER 0.2\n\n";

      expScript += "\t// Set the experiment value MFC flow setpoints\n";
      expScript += "\tMFC1_SETSP VAL_MFCSP1\n";
      expScript += "\tMFC2_SETSP VAL_MFCSP2\n";
      expScript += "\tMFC3_SETSP VAL_MFCSP3\n";
      expScript += "\t//MFC4_SETSP VAL_MFCSP4\n";
      expScript += "STEP_END\n\n";

      expScript += "STEP_PLL Machine Setup Part 2 \n";
      expScript += "\t// Start the MFC flows\n";
      expScript += "\tMFC1_SETVALVE TRUE\n";
      expScript += "\tMFC2_SETVALVE TRUE\n";
      expScript += "\tMFC3_SETVALVE TRUE\n";
      expScript += "\t//MFC4_SETVALVE TRUE\n\n";

      expScript += "\t// Set the experiment value water\n";
      expScript += "\t//WATER_PPM VAL_WATER\n\n";

      expScript += "\t// Set the experiment value pressure\n";
      expScript += "\tPRESSURE VAL_PRESSURE\n\n";
      expScript += "STEP_END\n\n";

      expScript += "STEP_PLL Pillar Prescan\n";
      expScript += "\t// Start a single raman scan\n";
      expScript += "\tRAMAN_SINGSCAN 30\n";
      expScript += "STEP_END\n\n";

      expScript += "STEP_SEQ Nanotube Growth\n";
      expScript += "\t// Start a continuous raman scan\n";
      expScript += "\tRAMAN_CONTSCAN 5\n\n";

      expScript += "\t// Set the experiment value temperature target\n";
      expScript += "\tTEMP_SETPOINT VAL_TEMP\n";
      expScript += "\t\n";

      expScript += "\t// Stop growth after some period\n";
      expScript += "\tWAIT 120\n\n";

      expScript += "\t// Stop the continuous Raman scan\n";
      expScript += "\tRAMAN_CONTSCAN FALSE\n";
      expScript += "\t\n";

      expScript += "\t// Stop the pillar temperature PID\n";
      expScript += "\tTEMP_SETPOINT FALSE\n\n";

      expScript += "\t// Set the laser to the postscan power\n";
      expScript += "\tLASER_POWER 0.2\n";
      expScript += "STEP_END\n\n";


      expScript += "STEP_PLL Pillar Postscan \n";
      expScript += "\t// Start a single raman scan\n";
      expScript += "\tRAMAN_SINGSCAN 30\n";
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
      interExpScript += "\t// Set the microscope to View Mode\n";
      interExpScript += "\tVIEWMODE TRUE\n\n";

      interExpScript += "\t// Let the microscope \"settle down\"\n";
      interExpScript += "\tWAIT 15\n";
      interExpScript += "\t\n";

      interExpScript += "\t// Goto the next pillar\n";
      interExpScript += "\tSTAGE_PILLAR NEXT\n";
      interExpScript += "STEP_END\n\n";

      return interExpScript;
    }
  }
}
