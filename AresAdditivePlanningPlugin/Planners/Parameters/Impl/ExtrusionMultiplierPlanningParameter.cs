using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class ExtrusionMultiplierPlanningParameter : AdditivePlanningParameter
  {
    public ExtrusionMultiplierPlanningParameter()
    {
      ScriptLabel = "VAL_EXTRUSION_MULTIPLIER";
      PythonLabel = "dispenser.multiplier";
    }


    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey(PythonLabel))
      {
        var parameter = toolpathParams[PythonLabel];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
