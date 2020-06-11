using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.Database.Tables.InnerContent;
using DynamicData.Annotations;

namespace ARESCore.Database.Tables
{
  [Table( "Planner", Schema = "public" )]
  public class PlannerEntity
  {
    [Key]
    public Guid Id { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.Now;

    public string PlannerType { get; set; } = "None";

    [CanBeNull]
    public Guid InputParameters { get; set; }
    
    [CanBeNull]
    public string InternalPlanningDbEntries { get; set; }
    public Guid[] PlanningDbEntries
    {
      get
      {
        return Array.ConvertAll( InternalPlanningDbEntries.Split( ';' ), v => Guid.Parse( v ) );
      }
      set
      {
        InternalPlanningDbEntries = String.Join( ";", value.Select( p => p.ToString() ).ToArray() );
      }
    }

    [CanBeNull]
    public PlannerOutputValues OutputValues { get; set; } = new PlannerOutputValues();

  }
}