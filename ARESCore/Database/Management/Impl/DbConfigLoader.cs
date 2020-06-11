using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Ninject;

namespace ARESCore.Database.Management.Impl
{
  internal class DbConfigLoader: IDbConfigLoader
  {
    public IDbConfig Load()
    {
      var config = AresKernel._kernel.Get<IDbConfig>();
      var cfg = new DbConfig();
      if ( File.Exists( "DbConfig.json" ) )
      {
        string txt = File.ReadAllText( "DbConfig.json" );
        cfg = JsonConvert.DeserializeObject<DbConfig>( txt );
        config.Ip = cfg.Ip;
        config.Port = cfg.Port;
      }
      else
      {
        cfg.Ip = "127.0.0.1";
        cfg.Port = 5432;
        config.Ip = cfg.Ip;
        config.Port = cfg.Port;
        Save();
      }

      return config;
    }

    public void Save()
    {
      var str = JsonConvert.SerializeObject( AresKernel._kernel.Get<IDbConfig>(), Formatting.Indented );
      File.WriteAllText( "DbConfig.json", str );
    }
  }
}
