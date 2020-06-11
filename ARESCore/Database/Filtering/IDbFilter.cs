using System.Collections.Generic;
using ARESCore.Database.Tables;

namespace ARESCore.Database.Filtering
{
  public interface IDbFilter<T>
  {
    IEnumerable<T> Filter( IEnumerable<T> preFilteredData );
    IEnumerable<ExperimentEntity> FilterExperimentsOn( IEnumerable<ExperimentEntity> expData, IEnumerable<T> typeData );

  }
}
