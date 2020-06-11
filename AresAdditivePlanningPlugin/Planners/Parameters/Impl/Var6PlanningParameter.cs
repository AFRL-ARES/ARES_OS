using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class Var6PlanningParameter : AdditivePlanningParameter
  {
    public Var6PlanningParameter()
    {
      ScriptLabel = "VAL_VAR6";
      PythonLabel = "uservars.var6";
    }


    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey("sic"))
      {
        var parameter = toolpathParams["six"];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
