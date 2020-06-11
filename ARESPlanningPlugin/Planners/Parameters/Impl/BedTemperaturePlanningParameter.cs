using AresAdditiveDevicesPlugin.Experiment;
using CommonServiceLocator;

namespace AresAdditivePlanningPlugin.Planners.Parameters.Impl
{
  public class BedTemperaturePlanningParameter : AdditivePlanningParameter
  {
    public BedTemperaturePlanningParameter()
    {
      ScriptLabel = "VAL_BED_TEMPERATURE";
      PythonLabel = "NOT YET IMPLEMENTED";
    }

    public override void ResetToToolpathValue()
    {
      var toolpathParams = ServiceLocator.Current.GetInstance<IToolpathParameters>();
      if (toolpathParams.ContainsKey("temperature"))
      {
        var parameter = toolpathParams["temperature"];
        Value = parameter.Value;
        Min = parameter.Min;
        Max = parameter.Max;
      }
    }
  }
}
