using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;

namespace ARESCore.PlanningSupport.Impl
{
  internal class SelectedPlannersRepository: ObservableCollectionExtended<IAresPlannerManager>, ISelectedPlannersRepository
  {
  }
}
