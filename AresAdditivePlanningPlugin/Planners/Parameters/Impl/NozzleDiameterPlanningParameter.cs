using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class NozzleDiameterPlanningParameter : AdditivePlanningParameter
  {
    public NozzleDiameterPlanningParameter()
    {
      ScriptLabel = "VAL_NOZZLE_DIAMETER";
      PythonLabel = "dispenser.diameter";
      
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
