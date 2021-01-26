using ARESCore.Configurations;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AresSampleDevicesPlugin.SampleDevice.Config
{
  public class SampleDeviceAppConfig : ReactiveSubscriber, ISampleDeviceAppConfig
  {


    public void Load()
    {
      var path = ServiceLocator.Current.GetInstance<IApplicationConfiguration>().CurrentAppConfigPath;
      path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
      var fullpath = path + "\\" + GetType().Name + ".json";
      if (File.Exists(fullpath))
      {
        var data = File.ReadAllText(fullpath);
        JsonConvert.DeserializeObject<SampleDeviceAppConfig>(data);
      }
    }

    public void Save()
    {
      var path = ServiceLocator.Current.GetInstance<IApplicationConfiguration>().CurrentAppConfigPath;
      path = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
      var fullpath = path + "\\" + GetType().Name + ".json";
      var data = JsonConvert.SerializeObject(this, Formatting.Indented);
      File.WriteAllText(fullpath, data);
    }
  }
}