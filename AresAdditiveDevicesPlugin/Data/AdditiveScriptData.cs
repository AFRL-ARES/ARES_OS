using System;
using ARESCore.DeviceSupport;

namespace AresAdditiveDevicesPlugin.Data
{
  public class AdditiveScriptData : IDeviceScriptData
  {
    public string ExpScript { get; set; }
    public string InterExpScript { get; set; }
    public string CampaignCloseScript { get; set; }

    public AdditiveScriptData()
    {
      ExpScript = DefaultExpScript();
      InterExpScript = DefaultInterExpScript();
      CampaignCloseScript = DefaultCampaignCloseScript();
    }

    public string DefaultExpScript()
    {
      string expScript = "// Default Additive Script, Generated at " + DateTime.Now.ToString("HH:mm:ss on MM/dd/yy") + "\n\n";

      expScript += "STEP_SEQ Move to First Available Experiment Cell\n";
      expScript += "\tSELECT_TOOL EXTRUDER\n";
      //expScript += "\tDELAY 250\n";
      expScript += "\tGOTO_AVAILABLE\n";
      expScript += "\tVALIDATE_AVAILABILITY\n";
      expScript += "STEP_END\n\n";

            expScript += "STEP_SEQ Assign Planned Parameter Values\n";
      // expScript += "STEP_PLL Assign Planned Parameter Values\n";
      expScript += "\tSET_NOZZLE_DIAMETER VAL_NOZZLE_DIAMETER\n";
      expScript += "\tSET_EXTRUSION_MULTIPLIER VAL_EXTRUSION_MULTIPLIER\n";
      expScript += "\tSET_TIP_HEIGHT VAL_TIP_HEIGHT\n";
      expScript += "\tSET_PRIME_DISTANCE VAL_PRIME_DISTANCE\n";
      expScript += "\tSET_PRIME_DELAY VAL_PRIME_DELAY\n";
      expScript += "\tSET_PRIME_RATE VAL_PRIME_RATE\n";
      expScript += "\tSET_RETRACT_DISTANCE VAL_RETRACT_DISTANCE\n";
      expScript += "\tSET_RETRACT_DELAY VAL_RETRACT_DELAY\n";
      expScript += "\tSET_RETRACT_RATE VAL_RETRACT_RATE\n";
      expScript += "\tSET_DISPENSE_SPEED VAL_DISPENSE_SPEED\n";
      expScript += "\t//SET_BED_TEMPERATURE VAL_BED_TEMPERATURE\n";
      expScript += "\tSET_VAR1 VAL_VAR1\n";
      expScript += "\tSET_VAR2 VAL_VAR2\n";
      expScript += "\tSET_VAR3 VAL_VAR3\n";
      expScript += "\tSET_VAR4 VAL_VAR4\n";
      expScript += "\tSET_VAR5 VAL_VAR5\n";
      expScript += "\tSET_VAR6 VAL_VAR6\n";
      expScript += "STEP_END\n\n";
      
      expScript += "STEP_SEQ Execute Toolpath\n";
      expScript += "\tSET_HOME\n";
      expScript += "\tGENERATE_TOOLPATH\n";
      expScript += "\tRUN_TOOLPATH\n";
      expScript += "STEP_END\n\n";
      
      expScript += "STEP_SEQ Perform Analysis\n";
      expScript += "\tSELECT_TOOL ALIGNMENT\n";
      expScript += "\tUPDATE_POSITIONS\n";
      expScript += "\tGOTO_ANALYSIS\n";
      expScript += "\tCAPTURE_ANALYSIS_IMAGE\n";
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
      interExpScript += "STEP_SEQ Mark Experiment Cell Invalid for Experimentation\n";
      interExpScript += "\tINVALIDATE_CELL\n";
      interExpScript += "STEP_END\n\n";
      return interExpScript;
    }
  }
}
