using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class Var1PlanningParameter : AdditivePlanningParameter
  {
    public Var1PlanningParameter()
    {
      ScriptLabel = "VAL_VAR1";
      PythonLabel = "uservars.var1";
    }

    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey("one"))
      {
        var parameter = toolpathParams["one"];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
