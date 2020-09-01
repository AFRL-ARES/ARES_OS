using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ARESCore.Database.Management.Impl;

namespace AresCNTDevicesPlugin.Laser.Database
{
  [Table( "MachineStateLaser", Schema = "public" )]
  [Persistent]
  public class LaserDbEntity
  {
    [Key]
    public Guid Id { get; set; }
    public Guid MachineStateId { get; set; }
    public double LaserPower { get; set; }
    public bool Shutter { get; set; }
  }
}