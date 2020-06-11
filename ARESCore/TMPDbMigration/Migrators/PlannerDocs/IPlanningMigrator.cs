using System;
using Newtonsoft.Json.Linq;

namespace ARESCore.TMPDbMigration.Migrators.PlannerDocs
{
  public interface IPlanningMigrator
  {
    Guid Migrate(JToken token);
  }
}