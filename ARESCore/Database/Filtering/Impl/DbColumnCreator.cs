using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.AnalysisSupport;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.Registries;
using Ninject;

namespace ARESCore.Database.Filtering.Impl
{
  class DbColumnCreator: IDbColumnCreator
  {
    public List<List<string>> GetRows( Type inputType )
    {
      if(( typeof( IPostProcessEntity )).IsAssignableFrom(inputType))
      {
        var analyzers = AresKernel._kernel.Get<IAresAnalyzerRegistry>();
        foreach (var analyzer in analyzers)
        {
          var res = analyzer.GetFilteredDataInRows( inputType );
          if ( res.Any() )
            return res.ToList();
        }
      }
      return new List<List<string>>();
    }
  }
}
