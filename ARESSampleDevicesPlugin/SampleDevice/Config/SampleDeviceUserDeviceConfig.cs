using System.IO;
using ARESCore.DisposePatternHelpers;
using ARESCore.UserSession;
using CommonServiceLocator;
using Newtonsoft.Json;
using ReactiveUI;

namespace AresSampleDevicesPlugin.SampleDevice.Config
{
  public class SampleDeviceUserDeviceConfig : ReactiveSubscriber, ISampleDeviceUserDeviceConfig
  {
    private StartupStateType _startupType;
    private double _doubleValue;

    public SampleDeviceUserDeviceConfig()
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
    public double DoubleValue
    {
      get => _doubleValue;
      set => this.RaiseAndSetIfChanged(ref _doubleValue, value);
    }

    public void Load()
    {
      var path = ServiceLocator.Current.GetInstance<ICurrentConfig>().User.SaveDirectory;
      var fullpath = path + "\\" + GetType().Name + ".json" + ".json";
      if (File.Exists(fullpath))
      {
        var data = File.ReadAllText(fullpath);
        var newData = JsonConvert.DeserializeObject<SampleDeviceUserDeviceConfig>(data);
        DoubleValue = newData.DoubleValue;
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