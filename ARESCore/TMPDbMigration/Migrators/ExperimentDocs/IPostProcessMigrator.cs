using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ARESCore.TMPDbMigration.Migrators.ExperimentDocs
{
  public interface IPostProcessMigrator
  {
    Guid Migrate( JToken token );
  }
}
