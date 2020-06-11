using ARESCore.Configurations;
using DynamicData.Binding;

namespace ARESCore.Registries
{
  public interface IConfigManagerRegistry : IObservableCollection<IConfigManager>
  {
    T GetManager<T>() where T : IConfigManager;
  }
}
