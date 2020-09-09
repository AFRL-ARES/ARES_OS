using ARESCore.AnalysisSupport;
using System.Windows.Controls;

namespace AresAnalysisPlugin.Management
{
  interface IAnalyzerSetterResolver
  {
    UserControl Resolve(IAresAnalyzer analyzer);
  }
}
