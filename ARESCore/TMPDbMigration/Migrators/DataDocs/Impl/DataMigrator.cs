using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.UI;
using Newtonsoft.Json.Linq;
using Ninject;

namespace ARESCore.TMPDbMigration.Migrators.DataDocs.Impl
{
  public class DataMigrator: IDataMigrator
  {
    private int _doneCount = 0;
    private int _assumedCount = 30509;
    public void Migrate( JToken jToken )
    {
      DataEntity de = new DataEntity();
      de.Id = Guid.Parse( jToken["_id"].Value<string>() );
      de.TimeStamp = jToken["CollectedAt_TimeStamp"]["$date"].Value<DateTime>();
      de.FromStep = new Guid( Convert.FromBase64String( jToken["FromStepId"]["$binary"].ToString() ) );
      foreach ( var analysisMigrator in AresKernel._kernel.GetAll<IAnalysisMigrator>() )
      {
       analysisMigrator.Migrate( jToken, de.Id );
      }
      AresKernel._kernel.Get<AresContext>().Data.Add( de );
      AresKernel._kernel.Get<AresContext>().SaveChanges();
      AresKernel._kernel.Get<IAresConsole>().WriteLine( "Finished Experiment " + ( _doneCount++ ) + " of " + _assumedCount + "(assumed)" );
    }

    public void Migrate( JToken jToken, Guid refId )
    {
      throw new NotImplementedException();
    }

    public string TypeString { get; }= "Data";
  }
}
