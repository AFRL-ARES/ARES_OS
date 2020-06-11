using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class Var3PlanningParameter : AdditivePlanningParameter
  {
    public Var3PlanningParameter()
    {
      ScriptLabel = "VAL_VAR3";
      PythonLabel = "uservars.var3";
    }

    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey("three"))
      {
        var parameter = toolpathParams["three"];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
