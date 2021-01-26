using ARESCore.DisposePatternHelpers;
using Ninject;
using ReactiveUI;

namespace ARESCore.UI.ViewModels
{
  public class AresSplashScreenViewModel : ReactiveSubscriber
  {
    public AresSplashScreenViewModel()
    {
      Status = AresKernel._kernel.Get<ILoadingStatus>();
      Status.StatusInfo = "Initializing...";
    }
    private ILoadingStatus _status;

    public ILoadingStatus Status
    {
      get => _status;
      set => this.RaiseAndSetIfChanged(ref _status, value);
    }
  }
}