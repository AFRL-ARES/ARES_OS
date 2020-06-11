using System.Collections.Generic;
using ARESCore.Database.Filtering;
using ARESCore.Database.Tables;
using ARESCore.DisposePatternHelpers;

namespace AresAdditiveDevicesPlugin.Database
{
    public class MachineStateFilter : BasicReactiveObjectDisposable, IDbFilter<MachineStateEntity>
    {
        public IEnumerable<MachineStateEntity> Filter(IEnumerable<MachineStateEntity> preFilteredData)
        {
            return preFilteredData;
        }

        public IEnumerable<ExperimentEntity> FilterExperimentsOn(IEnumerable<ExperimentEntity> expData, IEnumerable<MachineStateEntity> typeData)
        {
            return expData;
        }
    }
}
