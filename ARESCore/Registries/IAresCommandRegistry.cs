using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Commands;
using DynamicData.Binding;

namespace ARESCore.Registries
{
  public interface IAresCommandRegistry: IObservableCollection<IAresCommand>
  {
  }
}
