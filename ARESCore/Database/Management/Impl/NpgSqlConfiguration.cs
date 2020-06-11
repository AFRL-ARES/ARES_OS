using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Npgsql;

namespace ARESCore.Database.Management.Impl
{
  public class NpgSqlConfiguration : DbConfiguration
  {
    public NpgSqlConfiguration()
    {
      var name = "Npgsql";

      SetProviderFactory( providerInvariantName: name, providerFactory: NpgsqlFactory.Instance );

      SetProviderServices( providerInvariantName: name, provider: NpgsqlServices.Instance );

      SetDefaultConnectionFactory( connectionFactory: new NpgsqlConnectionFactory() );

    }
  }
}
