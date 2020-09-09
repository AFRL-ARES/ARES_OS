using System;

namespace AresAnalysisPlugin.Management
{
  interface IAnalyzerSetter
  {
    Type AnalyzerTypeSupported { get; set; }
  }
}
