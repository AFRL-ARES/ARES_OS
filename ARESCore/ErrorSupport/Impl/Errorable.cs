using System.Collections.Generic;
using System.Threading.Tasks;
using ARESCore.Experiment;
using Ninject;
using ReactiveUI;

namespace ARESCore.ErrorSupport.Impl
{
  public abstract class Errorable : ReactiveObject, IErrorable
  {
    private IAresError _error;

    protected Errorable()
    {
      AvailableErrorResponses.Add( ErrorResponse.Estop );
      AvailableErrorResponses.Add( ErrorResponse.Stop );
      AvailableErrorResponses.Add( ErrorResponse.IgnoreAndContinue );
      AvailableErrorResponses.Add( ErrorResponse.Retry );
    }

    public List<ErrorResponse> AvailableErrorResponses { get; } = new List<ErrorResponse>();

    public IAresError Error
    {
      get => _error;
      set
      {
        _error = value;
        this.RaisePropertyChanged();
      }
    }

    public virtual Task Handle( ErrorResponse response )
    {
      switch ( response )
      {
        case ErrorResponse.Estop:
          return HandleEStop();
        case ErrorResponse.IgnoreAndContinue:
          return HandleIgnoreAndContinue();
        case ErrorResponse.Retry:
          return HandleRetry();
        default:
          return HandleStop();
      }

    }

    protected virtual Task HandleIgnoreAndContinue()
    {
      return Task.CompletedTask;
    }

    protected virtual Task HandleRetry()
    {
      return Task.CompletedTask;
    }

    protected virtual Task HandleStop()
    {
      return Task.CompletedTask;
    }

    protected virtual Task HandleEStop()
    {
      AresKernel._kernel.Get<ICampaign>().InitiatingEStop = true; // Trigger the event for listeners
      return Task.CompletedTask;
    }

    protected void CreateAndAddErrorBundle( IErrorable handler, IErrorable notifier )
    {
      var bundle = AresKernel._kernel.Get<IErroredBundle>();
      bundle.ErrorHandler = handler;
      bundle.ErrorNotifier = notifier;
      AresKernel._kernel.Get<IErroredObjectRepo>().Add( bundle );
    }


  }
}
