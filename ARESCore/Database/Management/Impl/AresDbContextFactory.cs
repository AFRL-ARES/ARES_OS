using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Npgsql;

namespace ARESCore.Database.Management.Impl
{
  public class AresDbContextFactory:IDbContextFactory<AresContext>
  {
    public AresContext Create()
    {
      // Admittedly this is a little funky. We need the config file, but we also MUST call the constructor with the args
      // since there isn't a property to set it later. Thus, this static call and some Ninject games in the module.
      var connstr = "Server=127.0.0.1;Port=5432;Database=aresdatabase;" + "User Id=postgres;Password=a";
      var connection = new NpgsqlConnection( connstr );
      return new AresContext( connection );
    }
  }
}
