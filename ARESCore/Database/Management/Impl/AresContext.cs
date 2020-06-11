using System;
using System.Data.Entity;
using System.Linq;
using ARESCore.Database.Tables;
using Ninject;
using Npgsql;

namespace ARESCore.Database.Management.Impl
{
  [DbConfigurationType( typeof( NpgSqlConfiguration ) )]
  public class AresContext : DbContext
  {

    public AresContext( System.Data.Common.DbConnection conn ) : base( conn, true )
    {

    }

    protected override void OnModelCreating( DbModelBuilder modelBuilder )
    {
      var entityMethod = typeof( DbModelBuilder ).GetMethod( "Entity" );

      foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
      {
        var entityTypes = assembly
          .GetTypes()
          .Where( t =>
            t.GetCustomAttributes( typeof( PersistentAttribute ), inherit: true )
              .Any() );

        foreach ( var type in entityTypes )
        {
          entityMethod.MakeGenericMethod( type )
            .Invoke( modelBuilder, new object[] { } );
        }
      }
      base.OnModelCreating( modelBuilder );
    }

    public DbSet<Database.Tables.ExperimentEntity> Experiments { get; set; }
    public DbSet<Database.Tables.PlannerEntity> Planners { get; set; }
    public DbSet<Database.Tables.MachineStateEntity> MachineStates { get; set; }
    public DbSet<Database.Tables.DataEntity> Data { get; set; }
    public DbSet<ScriptStepCommandEntity> StepCommands { get; set; }
    public DbSet<ScriptStepEntity> ScriptSteps { get; set; }
    public DbSet<ScriptStepResultEntity> StepResults { get; set; }

    public static AresContext GetAresContext()
    {
      // Admittedly this is a little funky. We need the config file, but we also MUST call the constructor with the args
      // since there isn't a property to set it later. Thus, this static call and some Ninject games in the module.
      var config = AresKernel._kernel.Get<IDbConfig>();
      var connstr = "Server=" + config.Ip + ";Port=" + config.Port + ";Database=aresdatabase;" + "User Id=postgres;Password=a";
      var connection = new NpgsqlConnection( connstr );
      return new AresContext( connection );
    }
  }


}
