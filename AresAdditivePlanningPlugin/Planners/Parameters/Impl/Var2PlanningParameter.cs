using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class Var2PlanningParameter : AdditivePlanningParameter
  {
    public Var2PlanningParameter()
    {
      ScriptLabel = "VAL_VAR2";
      PythonLabel = "uservars.var2";
    }

    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey("two"))
      {
        var parameter = toolpathParams["two"];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
