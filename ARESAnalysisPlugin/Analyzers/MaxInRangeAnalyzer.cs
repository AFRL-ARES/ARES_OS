using ARESCore.AnalysisSupport;
using ARESCore.DisposePatternHelpers;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AresFCAnalysisPlugin.Analyzers
{
  class MaxInRangeAnalyzer : BasicReactiveObjectDisposable, IAresAnalyzer
  {
    public List<string> Headers { get; set; }
    public UserControl AnalysisDbFilter { get; set; }
    public string GetPostProcessOverview(IAresAnalyzer referenceProcess)
    {
      throw new NotImplementedException();
    }

    public void TrySet(string currentDesc, string lineToken)
    {
      throw new NotImplementedException();
    }

    public string Tokenize(string header)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> GetPrimaryAnalysisValues()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> GetSecondaryAnalysisValues()
    {
      throw new NotImplementedException();
    }

    public List<List<string>> GetFilteredDataInRows(Type inputType)
    {
      throw new NotImplementedException();
    }

    public Type DbTypeSupported { get; }
    public string AnalyzerName { get; set; } = "Max in Range";
    public bool IsSelected { get; set; }
  }
}
