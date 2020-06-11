using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class DispenserSpeedPlanningParameter : AdditivePlanningParameter
  {
    public DispenserSpeedPlanningParameter()
    {
      ScriptLabel = "VAL_DISPENSE_SPEED";
      PythonLabel = "dispenser.speed";

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
