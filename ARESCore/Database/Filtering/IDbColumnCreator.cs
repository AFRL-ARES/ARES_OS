using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Database.Filtering
{
  public interface IDbColumnCreator
  {
    List<List<string>> GetRows( Type inputType );
  }
}
