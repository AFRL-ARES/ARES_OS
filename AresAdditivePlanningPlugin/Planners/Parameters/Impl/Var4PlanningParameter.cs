using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class Var4PlanningParameter : AdditivePlanningParameter
  {
    public Var4PlanningParameter()
    {
      ScriptLabel = "VAL_VAR4";
      PythonLabel = "uservars.var4";
    }

    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey("four"))
      {
        var parameter = toolpathParams["four"];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
