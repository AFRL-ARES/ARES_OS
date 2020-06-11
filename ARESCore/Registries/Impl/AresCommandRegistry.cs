using ARESCore.Commands;
using DynamicData.Binding;

namespace ARESCore.Registries.Impl
{
  internal class AresCommandRegistry : ObservableCollectionExtended<IAresCommand>, IAresCommandRegistry
  {
  }
}
