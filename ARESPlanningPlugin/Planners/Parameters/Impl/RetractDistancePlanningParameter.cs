using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class RetractDistancePlanningParameter : AdditivePlanningParameter
  {
    public RetractDistancePlanningParameter()
    {
      ScriptLabel = "VAL_RETRACT_DISTANCE";
      PythonLabel = "dispenser.retract";
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
