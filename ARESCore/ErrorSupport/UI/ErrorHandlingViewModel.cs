using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using ARESCore.DisposePatternHelpers;
using ARESCore.ErrorSupport.Impl;
using ReactiveUI;

namespace ARESCore.ErrorSupport.UI
{
  public class ErrorHandlingViewModel : ReactiveSubscriber
  {
    private readonly IErroredObjectRepo _erroredObjectRepo;

    private IAresError _currentError;
    private IErroredBundle _currentErrorBundle;
    private Visibility _visibility;

    public ErrorHandlingViewModel( IErroredObjectRepo erroredObjectRepo )
    {
      _erroredObjectRepo = erroredObjectRepo;
      _erroredObjectRepo.CollectionChanged += ErroredRepoCountChanged; // Admitted, this is indirect. However, we still get a disposable so we don't have to worry about cleanup
      IgnoreAndContinueCommand = ReactiveCommand.Create<Unit, Task>( t => DoError( ErrorResponse.IgnoreAndContinue ) );
      RetryCommand = ReactiveCommand.Create<Unit, Task>( t => DoError( ErrorResponse.Retry ) );
      StopCommand = ReactiveCommand.Create<Unit, Task>( t => DoError( ErrorResponse.Stop ) );
      EstopCommand = ReactiveCommand.Create<Unit, Task>( t => DoError( ErrorResponse.Estop ) );
      Visibility = Visibility.Collapsed;
    }

    private Task DoError( ErrorResponse response )
    {

      var currentBundle = CurrentErrorBundle;
      _erroredObjectRepo.Remove( CurrentErrorBundle );
      if ( response == ErrorResponse.Stop || response == ErrorResponse.Estop )
      {
        // This handler is no longer in charge of the error, Disregard the remaining errors for this handler and let the bubble up occur
        _erroredObjectRepo.Clear();
      }
      return currentBundle.ErrorHandler.Handle( response );
    }

    private void ErroredRepoCountChanged( object sender, NotifyCollectionChangedEventArgs args )
    {
      if ( args.Action == NotifyCollectionChangedAction.Remove || args.Action == NotifyCollectionChangedAction.Reset )
      {
        // Get the next most recent error, or set to null
        CurrentErrorBundle = _erroredObjectRepo.LastOrDefault();
        return;
      }

      if ( CurrentErrorBundle == null )
      {
        // Get the first new error
        CurrentErrorBundle = _erroredObjectRepo.FirstOrDefault();
      }


    }

    public IErroredBundle CurrentErrorBundle
    {
      get => _currentErrorBundle;
      set
      {
        this.RaiseAndSetIfChanged( ref _currentErrorBundle, value );
        CurrentError = _currentErrorBundle?.ErrorNotifier.Error;
      }
    }

    public override void Dispose()
    {
      _erroredObjectRepo.CollectionChanged -= ErroredRepoCountChanged;
    }


    public IAresError CurrentError
    {
      get => _currentError;
      set
      {
        this.RaiseAndSetIfChanged( ref _currentError, value );
        if ( CurrentError == null )
        {
          Visibility = Visibility.Collapsed;
        }
        else
        {
          Visibility = Visibility.Visible;
        }
      }
    }

    public ReactiveCommand<Unit, Task> IgnoreAndContinueCommand { get; set; }

    public ReactiveCommand<Unit, Task> RetryCommand { get; set; }

    public ReactiveCommand<Unit, Task> StopCommand { get; set; }

    public ReactiveCommand<Unit, Task> EstopCommand { get; set; }

    public Visibility Visibility
    {
      get => _visibility;
      set => this.RaiseAndSetIfChanged( ref _visibility, value );
    }
  }
}
