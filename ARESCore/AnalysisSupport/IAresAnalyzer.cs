using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ARESCore.DisposePatternHelpers;
using ARESCore.PluginSupport;

namespace ARESCore.AnalysisSupport
{
  public interface IAresAnalyzer : IAresPlugin, IBasicReactiveObjectDisposable
  {
    List<string> Headers { get; set; }

    UserControl AnalysisDbFilter { get; set; }
    string GetPostProcessOverview(IAresAnalyzer referenceProcess);
    void TrySet(string currentDesc, string lineToken);
    string Tokenize(string header);
    IEnumerable<string> GetPrimaryAnalysisValues();
    IEnumerable<string> GetSecondaryAnalysisValues();
    List<List<string>> GetFilteredDataInRows(Type inputType);

    Type DbTypeSupported { get; }

    string AnalyzerName { get; set; }
    bool IsSelected { get; set; }
  }
}
