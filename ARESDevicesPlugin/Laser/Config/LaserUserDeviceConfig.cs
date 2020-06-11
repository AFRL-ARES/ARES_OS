using System.IO;
using ARESCore.DisposePatternHelpers;
using ARESCore.UserSession;
using CommonServiceLocator;
using Newtonsoft.Json;
using ReactiveUI;

namespace ARESDevicesPlugin.Laser.Config
{
  public class LaserUserDeviceConfig : BasicReactiveObjectDisposable, ILaserUserDeviceConfig
  {
    private StartupStateType _startupType;
    private double _laserPower;

    public LaserUserDeviceConfig()
    {
      StartupType = StartupStateType.DO_NOTHING;
    }

    [JsonProperty]
    public StartupStateType StartupType
    {
      get => _startupType;
      set => this.RaiseAndSetIfChanged(ref _startupType, value);
    }

    [JsonProperty]
    public double LaserPower
    {
      get => _laserPower;
      set => this.RaiseAndSetIfChanged(ref _laserPower, value);
    }

    public void Load()
    {
      var path = ServiceLocator.Current.GetInstance<ICurrentConfig>().User.SaveDirectory;
      var fullpath = path + "\\" + GetType().Name + ".json" + ".json";
      if (File.Exists(fullpath))
      {
        string data = File.ReadAllText(fullpath);
        var newData = JsonConvert.DeserializeObject<LaserUserDeviceConfig>(data);
        LaserPower = newData.LaserPower;
      }
    }

    public void Save()
    {
      var path = ServiceLocator.Current.GetInstance<ICurrentConfig>().User.SaveDirectory;
      var fullpath = path + "\\" + GetType().Name + ".json";
      var data = JsonConvert.SerializeObject(this, Formatting.Indented);
      File.WriteAllText(fullpath, data);
    }
  }
}