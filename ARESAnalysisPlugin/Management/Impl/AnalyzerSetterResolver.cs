using ARESCore.AnalysisSupport;
using CommonServiceLocator;
using System.Linq;
using System.Windows.Controls;

namespace AresAnalysisPlugin.Management.Impl
{
  internal class AnalyzerSetterResolver : IAnalyzerSetterResolver
  {

    public UserControl Resolve(IAresAnalyzer analyzer)
    {
      return ServiceLocator.Current.GetAllInstances<IAnalyzerSetter>().FirstOrDefault(s => s.AnalyzerTypeSupported == analyzer.GetType()) as UserControl;
    }
  }
}
