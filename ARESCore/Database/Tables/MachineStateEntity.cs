using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DynamicData.Annotations;


namespace ARESCore.Database.Tables
{
  [Table( "MachineState", Schema = "public")]
  public class MachineStateEntity
  {
    [Key]
    public  Guid Id { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.Now;

    [CanBeNull]
    public  Guid FromStep { get; set; }
  }
}