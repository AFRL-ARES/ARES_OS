using System.Collections.Generic;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using DynamicData.Binding;

namespace AresAdditiveDevicesPlugin.Processing
{
    public interface
      IComponent : IBasicReactiveObjectDisposable
    {
        /// <summary>
        /// Component Id
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Component Name
        /// </summary>
        string ComponentName { get; set; }

        /// <summary>
        /// flag to know if component has no inputs and can run
        /// by itself
        /// </summary>
        bool CanAutoStart { get; }

        /// <summary>
        /// Does the work
        /// </summary>  
        Task StartComponent(IList<IProcessData> inputs);
        //    Task StartComponent( IRepository<IProcessData> inputs );
        //    Task SartComponent { get; set; }

        /// <summary>
        /// If process is complete
        /// </summary>
        bool IsComplete { get; }
        IProcessData Output { get; }
        IObservableCollection<IProcessData> DefaultInputs { get; set; }

    }
}
