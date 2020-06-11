using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ARESCore.Database.Tables.InnerContent;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables
{
  [Table( "Experiments", Schema = "public" )]
  public class ExperimentEntity
  {

    [Key]
    public Guid Id { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.Now;

    [CanBeNull]
    public string Project { get; set; }
    [CanBeNull]
    public string OriginalWorkingFolder { get; set; }

    public bool Imported { get; set; } = false;
    [CanBeNull]
    public string Notes { get; set; }

    [CanBeNull]
    public ExperimentData ExperimentData { get; set; } = new ExperimentData();

    [CanBeNull]
    public Guid PostProcessData { get; set; }

    [CanBeNull]
    public Guid Planner { get; set; }

    [CanBeNull]
    public string InternalData { get; set; }
    public Guid[] Data
    {
      get
      {
        return Array.ConvertAll( InternalData.Split( ';' ), v => Guid.Parse(v) );
      }
      set
      {
        InternalData = String.Join( ";", value.Select( p => p.ToString() ).ToArray() );
      }
    }

    [CanBeNull]
    public string InternalMachineStates { get; set; }
    public Guid[] MachineStates
    {
      get
      {
        return Array.ConvertAll( InternalMachineStates.Split( ';' ), v => Guid.Parse( v ) );
      }
      set
      {
        InternalMachineStates = String.Join( ";", value.Select( p => p.ToString() ).ToArray() );
      }
    }

    [CanBeNull]
    public string InternalScriptSteps { get; set; }
    public Guid[] ScriptSteps
    {
      get
      {
        return Array.ConvertAll( InternalScriptSteps.Split( ';' ), v => Guid.Parse( v ) );
      }
      set
      {
        InternalScriptSteps = String.Join( ";", value.Select( p => p.ToString() ).ToArray() );
      }
    }

    [CanBeNull]
    public string InternalStepResults { get; set; }
    public Guid[] StepResults
    {
      get
      {
        return Array.ConvertAll( InternalStepResults.Split( ';' ), v => Guid.Parse( v ) );
      }
      set
      {
        InternalStepResults = String.Join( ";", value.Select( p => p.ToString() ).ToArray() );
      }
    }

    [CanBeNull]
    public ExperimentExecutionResults ExperimentExecutionResult { get; set; } = new ExperimentExecutionResults();

  }
}
