using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARESCore.Configurations;
using ARESCore.Configurations.impl;
using CommonServiceLocator;
using Newtonsoft.Json;

namespace AresCNTDevicesPlugin.Laser.Config
{
  public class LaserAppConfig : SerialPortConfig, ILaserAppConfig
  {
   

    public void Load()
    {
      var path = ServiceLocator.Current.GetInstance<IApplicationConfiguration>().CurrentAppConfigPath;
      path = path.Substring( 0, path.LastIndexOf( "\\" ) );
      var fullpath = path + "\\" + GetType().Name + ".json";
      if ( File.Exists( fullpath ) )
      {
        string data = File.ReadAllText( fullpath );
        var newData = JsonConvert.DeserializeObject<LaserAppConfig>( data );
        LoadSerial( newData );
      }
    }

    public void Save()
    {
      var path = ServiceLocator.Current.GetInstance<IApplicationConfiguration>().CurrentAppConfigPath;
      path = path.Substring( 0, path.LastIndexOf( "\\" ) );
      var fullpath = path + "\\" + GetType().Name + ".json";
      var data = JsonConvert.SerializeObject( this, Formatting.Indented );
      File.WriteAllText( fullpath, data );
    }
  }
}