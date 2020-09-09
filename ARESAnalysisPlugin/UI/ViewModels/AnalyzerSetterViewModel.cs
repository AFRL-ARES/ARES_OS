using ARESCore.AnalysisSupport;
using ARESCore.DisposePatternHelpers;

namespace AresAnalysisPlugin.UI.ViewModels
{
  class AnalyzerSetterViewModel : BasicReactiveObjectDisposable
  {
    public IAresAnalyzer Analyzer { get; set; }
  }
}
