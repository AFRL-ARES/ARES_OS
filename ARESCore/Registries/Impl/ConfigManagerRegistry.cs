using System.Linq;
using ARESCore.Configurations;
using DynamicData.Binding;

namespace ARESCore.Registries.Impl
{
  internal class ConfigManagerRegistry : ObservableCollectionExtended<IConfigManager>, IConfigManagerRegistry
  {
    public T GetManager<T>() where T : IConfigManager
    {
      return (T)this.FirstOrDefault( manager => manager is T );
    }
  }
}
