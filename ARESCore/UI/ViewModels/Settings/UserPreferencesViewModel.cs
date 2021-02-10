using ARESCore.DisposePatternHelpers;
using ARESCore.Registries;
using ReactiveUI;

namespace ARESCore.UI.ViewModels.Settings
{
  public class UserPreferencesViewModel: ReactiveSubscriber
  {
    private IConfigManagerRegistry _registry;

    public UserPreferencesViewModel( IConfigManagerRegistry registry)
    {
      Registry = registry;
    }

    public UserPreferencesViewModel()
    {
    }

    public IConfigManagerRegistry Registry
    {
      get => _registry;
      set => this.RaiseAndSetIfChanged( ref _registry, value );
    }
  }
}
