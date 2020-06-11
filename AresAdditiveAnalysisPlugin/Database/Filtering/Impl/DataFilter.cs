using ARESCore.Database.Filtering;
using ARESCore.Database.Tables;
using System.Collections.Generic;
using System.Linq;

namespace AresAdditiveAnalysisPlugin.Database.Filtering.Impl
{
    public class DataFilter : IDbFilter<DataEntity>
    {
        public IEnumerable<DataEntity> Filter(IEnumerable<DataEntity> preFilteredData)
        {
            return preFilteredData;
        }

        public IEnumerable<ExperimentEntity> FilterExperimentsOn(IEnumerable<ExperimentEntity> expData, IEnumerable<DataEntity> typeData)
        {
            return from eDoc in expData select eDoc;
        }
    }
}
