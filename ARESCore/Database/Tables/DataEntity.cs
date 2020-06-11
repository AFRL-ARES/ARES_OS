using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ARESCore.Database.Management.Impl;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables
{
  [Table("Data", Schema = "public")]
  public class DataEntity
  {
    [Key]
    public Guid Id { get; set; }
    [NotNull]
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    public Guid FromStep { get; set; }
  }
}