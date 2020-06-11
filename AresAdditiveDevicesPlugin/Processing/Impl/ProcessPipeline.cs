using System.Linq;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Processing.Staging;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.Processing.Impl
{
    public class ProcessPipeline : ObservableCollectionExtended<IStageEntry>
    {

        public async Task Execute()
        {
            for (int i = 1; i <= this.Max(entry => entry.StageId); i++)
            {
                var stage = this.Where(entry => entry.StageId == i);
                var tasks = stage.Select(entry => entry.ExecuteProcess());
                await Task.WhenAll(tasks);
            }
        }

    }
}
