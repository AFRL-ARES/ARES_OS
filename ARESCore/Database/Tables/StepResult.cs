using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables
{
  [Table( "StepResults", Schema = "public" )]
  public class ScriptStepResultEntity
  {
    [Key]
    public Guid Id { get; set; }

    public DateTime Started { get; set; } = DateTime.MaxValue;

    public TimeSpan Runtime { get; set; } = TimeSpan.Zero;

    public Guid StepId { get; set; } = Guid.Empty;

  }
}
