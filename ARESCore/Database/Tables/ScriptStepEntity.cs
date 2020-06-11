using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables
{
  [Table( "ScriptSteps", Schema = "public" )]
  public class ScriptStepEntity
  {
    [Key]
    public Guid Id { get; set; }

    public string StepName { get; set; } = "Default";

    public bool SequentialExecution { get; set; } = true;

    public string StepText { get; set; } = "Nothing";

    [CanBeNull]
    public string InternalStepCommands { get; set; }
    public Guid[] StepCommands
    {
      get
      {
        return Array.ConvertAll( InternalStepCommands.Split( ';' ), v => Guid.Parse( v ) );
      }
      set
      {
        InternalStepCommands = String.Join( ";", value.Select( p => p.ToString() ).ToArray() );
      }
    }

  }
}
