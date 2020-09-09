using ARESCore.AnalysisSupport;
using DynamicData.Binding;

namespace AresAnalysisPlugin.Management
{
  interface IPotentialAnalyzerRegistry : IObservableCollection<IAresAnalyzer>
  {
  }
}
