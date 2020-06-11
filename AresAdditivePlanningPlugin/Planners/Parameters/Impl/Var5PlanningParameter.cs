using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class Var5PlanningParameter : AdditivePlanningParameter
  {
    public Var5PlanningParameter()
    {
      ScriptLabel = "VAL_VAR5";
      PythonLabel = "uservars.var5";
    }


    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey("five"))
      {
        var parameter = toolpathParams["five"];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
