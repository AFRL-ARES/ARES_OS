using ARESCore.AnalysisSupport;
using ARESCore.DisposePatternHelpers;

namespace AresFCAnalysisPlugin.UI.ViewModels
{
  class AnalyzerSetterViewModel : BasicReactiveObjectDisposable
  {
    public IAresAnalyzer Analyzer { get; set; }
  }
}
