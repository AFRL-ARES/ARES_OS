using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Database.Tables;
using DynamicData.Binding;

namespace ARESCore.Registries
{
  public interface IMachineStateRegistry : IObservableCollection<IMachineState>
  {
    int MsBetweenWrites { get; set; }
  }
}
