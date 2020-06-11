using ARESCore.DisposePatternHelpers;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.Events.Impl
{
  public class EventHub : BasicDisposable
  {
    private readonly Subject<object> _subject = new Subject<object>();

    public EventHub()
    {
      Disposables.Add(_subject);
    }

    public IObservable<T> GetEvent<T>() where T : IEventAction
    {
      return _subject.ObserveOn(TaskPoolScheduler.Default).OfType<T>();
    }

    public Task Publish<T>(T anEvent) where T : IEventAction
    {
      _subject.OnNext(anEvent);
      return Task.CompletedTask;
    }
  }
}
