using ARESCore.AnalysisSupport;
using DynamicData.Binding;

namespace AresFCAnalysisPlugin.Management
{
  interface IPotentialAnalyzerRegistry : IObservableCollection<IAresAnalyzer>
  {
  }
}
