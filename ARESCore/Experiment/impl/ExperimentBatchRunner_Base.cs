using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.Experiment.impl
{
    public abstract class ExperimentBatchRunner_Base : BasicReactiveObjectDisposable
    {
        private IExperimentBatch _experimentBatch;

        public ExperimentBatchRunner_Base(IExperimentBatch expBatch)
        {
            ExperimentBatch = expBatch;
        }

        public abstract Task RunExperimentBatchAsync(int expIndexStart, int batchSize);

        public IExperimentBatch ExperimentBatch
        {
          get { return _experimentBatch; }
          protected set { _experimentBatch = value; }
        }
  }
}
