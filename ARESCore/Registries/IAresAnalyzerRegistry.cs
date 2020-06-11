using System;
using System.Collections.Generic;
using ARESCore.AnalysisSupport;
using DynamicData.Binding;

namespace ARESCore.Registries
{
  public interface IAresAnalyzerRegistry: IObservableCollection<IAresAnalyzer>
  {
  }
}
