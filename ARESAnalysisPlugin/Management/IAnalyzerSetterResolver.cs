using ARESCore.AnalysisSupport;
using System.Windows.Controls;

namespace AresFCAnalysisPlugin.Management
{
  interface IAnalyzerSetterResolver
  {
    UserControl Resolve(IAresAnalyzer analyzer);
  }
}
