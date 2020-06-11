using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARESCore.TMPDbMigration.Migrators
{
  public interface ICollectionMigrator : IMigrator
  {
    Task MigrateFile( string file,bool doParallel);
    bool Migrating { get; set; }
  }
}
