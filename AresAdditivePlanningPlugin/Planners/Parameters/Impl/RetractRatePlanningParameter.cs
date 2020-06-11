using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class RetractRatePlanningParameter : AdditivePlanningParameter
  {
    public RetractRatePlanningParameter()
    {
      ScriptLabel = "VAL_RETRACT_RATE";
      PythonLabel = "dispenser.retract_rate";
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
