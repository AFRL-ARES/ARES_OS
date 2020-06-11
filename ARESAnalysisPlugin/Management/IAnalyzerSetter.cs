using System;

namespace AresFCAnalysisPlugin.Management
{
  interface IAnalyzerSetter
  {
    Type AnalyzerTypeSupported { get; set; }
  }
}
