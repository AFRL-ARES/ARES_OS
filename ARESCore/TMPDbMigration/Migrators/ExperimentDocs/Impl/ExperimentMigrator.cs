using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.Database.Tables.InnerContent;
using ARESCore.UI;
using Newtonsoft.Json.Linq;
using Ninject;

namespace ARESCore.TMPDbMigration.Migrators.ExperimentDocs.Impl
{
  public class ExperimentMigrator : IExperimentMigrator
  {
    private int _doneCount = 0;
    private int _assumedCount = 1909;
    public void Migrate( JToken jToken )
    {
      ExperimentEntity ee = new ExperimentEntity();
      ee.Id = Guid.Parse( jToken["_id"].Value<string>() );
      ee.TimeStamp = jToken["Started_TimeStamp"]["$date"].Value<DateTime>();
      ee.Project = jToken["Project"].Value<string>();
      ee.OriginalWorkingFolder = jToken["OriginalWorkingFolder"].Value<string>();
      ee.Imported = jToken["WasImported"].Value<bool>();
      ee.Notes = jToken["UserNotes"].Value<string>();
      ee.ExperimentData = ParseExpData( jToken["ExperimentData"] );
      ee.Planner = new Guid( Convert.FromBase64String( jToken["ChosenPlanner_Doc_id"]["$binary"].ToString() ) );
      ee.Data = ParseGuids( jToken["RamanData_Doc_id"] );
      ee.MachineStates = ParseGuids( jToken["MachineStates_Doc_id"] );
      ee.ScriptSteps = ParseScriptSteps( jToken["ExpScriptSteps"] );
      if ( jToken["ExperimentExecutionResults"].HasValues )
      {
        ee.ExperimentExecutionResult = ParseExecutionResult( jToken["ExperimentExecutionResults"] );
        ee.StepResults = ParseStepResults( jToken["ExperimentExecutionResults"]["StepResults"] );
      }
      foreach ( var postProcessMigrator in AresKernel._kernel.GetAll<IPostProcessMigrator>() )
      {
        var guid = postProcessMigrator.Migrate( jToken["PostProcessData"] );
        if(!guid.Equals(Guid.Empty))
        {
          ee.PostProcessData = guid;
        }
      }
      AresKernel._kernel.Get<AresContext>().Experiments.Add( ee );
      AresKernel._kernel.Get<AresContext>().SaveChanges();
      AresKernel._kernel.Get<IAresConsole>().WriteLine( "Finished Experiment " + ( _doneCount++ ) + " of " + _assumedCount + "(assumed)");
    }

    public void Migrate( JToken jToken, Guid refId )
    {
      // nah
    }

    private Guid[] ParseStepResults( JToken jToken )
    {
      var guids = new List<Guid>();
      foreach ( var stepRes in jToken )
      {
        var sr = new ScriptStepResultEntity();
        sr.Id = Guid.NewGuid();
        guids.Add( sr.Id );
        sr.Runtime = TimeSpan.Parse( stepRes["Runtime"].Value<string>() );
        sr.Started = stepRes["Started"]["$date"].Value<DateTime>();
        sr.StepId = new Guid( Convert.FromBase64String( stepRes["StepId"]["$binary"].ToString() ) );
        AresKernel._kernel.Get<AresContext>().StepResults.Add(sr);
      }
      return guids.ToArray();
    }

    private ExperimentExecutionResults ParseExecutionResult( JToken jToken )
    {
      ExperimentExecutionResults eer = new ExperimentExecutionResults();

      eer.Runtime = TimeSpan.Parse(jToken["Runtime"].Value<string>());
      eer.Started = jToken["Started"]["$date"].Value<DateTime>();
      return eer;

    }

    private Guid[] ParseScriptSteps( JToken jToken )
    {
      var guids = new List<Guid>();
      foreach ( var scriptStep in jToken )
      {
        ScriptStepEntity sse = new ScriptStepEntity();
        var guid = new Guid( Convert.FromBase64String( scriptStep["StepId"]["$binary"].ToString() ) );
        if ( guid == null )
          guid = Guid.NewGuid();
        sse.Id = guid;
        guids.Add( guid );
        sse.StepName = scriptStep["StepName"].Value<string>();
        sse.SequentialExecution = scriptStep["SequentialExecution"].Value<bool>();
        sse.StepText = scriptStep["StepText"].Value<string>();
        List<Guid> sscGuids = new List<Guid>();
        foreach ( var stepCommand in scriptStep["StepCommands"] )
        {
          ScriptStepCommandEntity ssc = new ScriptStepCommandEntity();
          ssc.CommandText = stepCommand["OriginalCommandText"].Value<string>();
          var sscGuid = Guid.NewGuid();
          ssc.Id = sscGuid;
          sscGuids.Add( sscGuid );
          AresKernel._kernel.Get<AresContext>().StepCommands.Add( ssc );
        }
        sse.StepCommands = sscGuids.ToArray();
        if(!AresKernel._kernel.Get<AresContext>().ScriptSteps.Any(s => s.Id.Equals(guid)))
          AresKernel._kernel.Get<AresContext>().ScriptSteps.Add( sse );
      }
      return guids.ToArray();
    }

    private Guid[] ParseGuids( JToken jToken )
    {
      List<Guid> guids = new List<Guid>();
      foreach ( var token in jToken )
      {
        var guid = new Guid( Convert.FromBase64String( token["$binary"].ToString() ) );
        guids.Add( guid );
      }
      return guids.ToArray();
    }

    private ExperimentData ParseExpData( JToken jToken )
    {
      ExperimentData ed = new ExperimentData();
      List<string> descList = new List<string>();
      List<double> dataList = new List<double>();
      foreach ( var token in jToken["Desc"] )
      {
        descList.Add( token.Value<string>() );
      }
      foreach ( var token in jToken["Data"] )
      {
        dataList.Add( token.Value<double>() );
      }
      ed.Descs = descList.ToArray();
      ed.Data = dataList.ToArray();
      return ed;
    }

    public string TypeString { get; } = "";
  }
}