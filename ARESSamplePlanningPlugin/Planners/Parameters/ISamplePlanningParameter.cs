namespace AresSamplePlanningPlugin.Planners.Parameters
{
  public interface ISamplePlanningParameter
  {
    double Value { get; set; }
    bool IsPlanned { get; set; }
    string ScriptLabel { get; }
  }
}