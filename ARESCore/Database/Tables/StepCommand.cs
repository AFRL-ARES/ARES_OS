using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables
{
  [Table( "StepCommands", Schema = "public" )]
  public class ScriptStepCommandEntity
  {
    [Key]
    public Guid Id { get; set; }

    public string CommandText { get; set; } = "Default";

  }
}
