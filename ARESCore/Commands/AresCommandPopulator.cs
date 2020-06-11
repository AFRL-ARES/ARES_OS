using ARESCore.Experiment.Scripting.Commands;
using ARESCore.Registries;
using Ninject;

namespace ARESCore.Commands
{
  internal class AresCommandPopulator : IAresCommandPopulator
  {
    public void Populate()
    {
      var repo = AresKernel._kernel.Get<IAresCommandRegistry>();
      var sec = new StepEndCommand();
      var sspc = AresKernel._kernel.Get<StepStartParallelCommand>();
      var sssc = AresKernel._kernel.Get<StepStartSequentialCommand>();
      var wc = new WaitCommand();
      var dc = new DelayCommand();
      repo.Add(sec);
      repo.Add(sspc);
      repo.Add(sssc);
      repo.Add(wc);
      repo.Add(dc);
    }
  }
}
