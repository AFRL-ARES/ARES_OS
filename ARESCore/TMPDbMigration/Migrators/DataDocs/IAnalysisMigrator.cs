using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ARESCore.TMPDbMigration.Migrators.DataDocs
{
  public interface IAnalysisMigrator
  {
    void Migrate( JToken token, Guid refId );
  }
}
