using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DynamicData.Annotations;

namespace ARESCore.Experiment
{
  [Table( "Experiments", Schema = "public" )]
  public class Experiment
  {
    [Key]
    public Guid Id { get; set; }
    
    public DateTime TimeStamp { get; set; }

    [CanBeNull]
    public string Project { get; set; }
    [CanBeNull]
    public string OriginalWorkingFolder { get; set; }

    public bool Imported { get; set; } = false;
    [CanBeNull]
    public string Notes { get; set; }



  }
}
