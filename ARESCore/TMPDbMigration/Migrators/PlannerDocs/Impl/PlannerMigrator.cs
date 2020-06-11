using ARESCore.Database.Management.Impl;
using ARESCore.UI;
using Newtonsoft.Json.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using ARESCore.Database.Tables;
using ARESCore.Database.Tables.InnerContent;

namespace ARESCore.TMPDbMigration.Migrators.PlannerDocs.Impl
{
  public class PlannerMigrator : IPlannerMigrator
  {
    private int _doneCount = 0;
    private int _assumedCount = 1909;

    public void Migrate(JToken jToken)
    {
      var pe = new PlannerEntity()
      {
        Id = Guid.Parse(jToken["_id"].Value<string>()),
        TimeStamp = jToken["Created_TimeStamp"]["$date"].Value<DateTime>(),
        PlannerType = jToken["PlannerType"].Value<string>()
      };

      if (jToken["OutputValues"].HasValues)
      {
        pe.OutputValues = ParseOutputValues(jToken["OutputValues"]);
      }

      if (jToken["InputParameters"].HasValues)
      {
        pe.PlanningDbEntries = ParseDbValues(jToken["InputParameters"]["PlanningDatabase"]);
        pe.InputParameters = ParseInputValues(jToken["InputParameters"]);
      }
      AresKernel._kernel.Get<AresContext>().Planners.Add(pe);
      AresKernel._kernel.Get<AresContext>().SaveChanges();
      AresKernel._kernel.Get<IAresConsole>().WriteLine("Finished Experiment " + (_doneCount++) + " of " + _assumedCount + "(assumed)");

    }


    public void Migrate(JToken jToken, Guid refId)
    {
      // nah
    }

    private PlannerOutputValues ParseOutputValues(JToken jToken)
    {
      var pov = new PlannerOutputValues();
      var descList = new List<string>();
      var dataList = new List<double[]>();

      foreach (var token in jToken["Desc"])
      {
        descList.Add(token.Value<string>());
      }

      foreach (var token in jToken["Data"])
      {
        var innerDoubles = new List<double>();
        foreach (var innerToken in token)
        {
          innerDoubles.Add(innerToken.Value<double>());
        }

        dataList.Add(innerDoubles.ToArray());
      }

      pov.Descs = descList.ToArray();
      pov.Data = dataList;
      return pov;
    }

    private Guid[] ParseDbValues(JToken jToken)
    {
      var guidList = new List<Guid>();
      foreach (var token in jToken)
      {
        guidList.Add(Guid.Parse(token["_id"].Value<string>()));
      }

      return guidList.ToArray();
    }
    private Guid ParseInputValues(JToken jToken)
    {
      foreach (var planningMigrator in AresKernel._kernel.GetAll<IPlanningMigrator>())
      {
        var guid = planningMigrator.Migrate(jToken);
        if (!guid.Equals(Guid.Empty))
        {
          return guid;
        }
      }
      return Guid.Empty;
    }

    public string TypeString { get; } = "Planner";


  }
}