using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using ARESCore.TMPDbMigration.Migrators.DataDocs;
using ARESCore.TMPDbMigration.Migrators.ExperimentDocs;
using ARESCore.TMPDbMigration.Migrators.MachineStateDocs;
using ARESCore.TMPDbMigration.Migrators.PlannerDocs;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace ARESCore.TMPDbMigration.Migrators.Impl
{
  public class CollectionMigrator : ReactiveSubscriber, ICollectionMigrator
  {
    private Dictionary<string, IMigrator> _migratorMap = new Dictionary<string, IMigrator>();
    private bool _migrating = true;

    public CollectionMigrator( IMachineStateMigrator machineStateMigrator, IExperimentMigrator experimentMigrator, IDataMigrator dataMigrator, IPlannerMigrator plannerMigrator )
    {
      _migratorMap.Add( "MachineState_Values", machineStateMigrator );
      _migratorMap.Add( "Experiment", experimentMigrator );
      _migratorMap.Add( "Data", dataMigrator );
      _migratorMap.Add( "Planner", plannerMigrator);
    }

    public async Task MigrateFile( string file, bool doParallel )
    {
      await Task.Run( () =>
      {
        if ( doParallel )
        {
          // Negate logic because we're going to throw this code away anyway
          Migrating = false;
          Parallel.ForEach( File.ReadLines( file ), ( line, _, lineNumber ) =>
          {
            //TODO FIXME Process object
            var jToken = JToken.Parse( line );
            Migrate( jToken);
          } );
        }
        else
        {
          foreach ( var line in File.ReadLines(file) )
          {
            var jToken = JToken.Parse( line );
            Migrate( jToken );
          }
        }
      } );

      Migrating = true;
    }

    // Entire file string
    public void Migrate( JToken jToken)
    {
      if ( jToken["MachineState_Values"] != null && jToken["MachineState_Values"].HasValues )
      {
        _migratorMap["MachineState_Values"].Migrate( jToken);
      }
      if(jToken["Started_TimeStamp"] != null && jToken["Started_TimeStamp"].HasValues )
      {
        _migratorMap["Experiment"].Migrate( jToken);
      }
      if (jToken["Created_TimeStamp"] != null && jToken["Created_TimeStamp"].HasValues)
      {
        _migratorMap["Planner"].Migrate(jToken);
      }

      if (jToken["FromStepId"] != null && jToken["FromStepId"].HasValues)
      {
        _migratorMap["Data"].Migrate(jToken);
      }

    }

    public void Migrate( JToken jToken, Guid refId )
    {
      // nope
    }

    public string TypeString { get; } = string.Empty;

    public bool Migrating
    {
      get => _migrating;
      set => this.RaiseAndSetIfChanged( ref _migrating, value );
    }
  }
}
