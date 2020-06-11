namespace AresAdditivePlanningPlugin.Planners.Parameters
{
  public interface IAdditivePlanningParameter
  {
    string ScriptLabel { get; }
    string PythonLabel { get; }
    double Value { get; set; }
    bool IsPlanned { get; set; }
    double Min { get; set; }
    double Max { get; set; }
    void ResetToToolpathValue();
  }
}
