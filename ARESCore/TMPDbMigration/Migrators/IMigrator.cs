using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ARESCore.TMPDbMigration.Migrators
{
  public interface IMigrator
  {
    void Migrate( JToken jToken);
    void Migrate( JToken jToken, Guid refId );
    string TypeString { get; }
  }
}