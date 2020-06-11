using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ARESCore.PlanningSupport
{
  public interface IPlanningParameters
  {
    IAresPlanningRequest GenerateRequest( );
  }
}