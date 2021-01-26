using System.Reactive;
using ARESCore.DisposePatternHelpers;
using AresSampleDevicesPlugin.SampleDevice.Config;
using ReactiveUI;

namespace AresSampleDevicesPlugin.SampleDevice.UI.Config
{
  public class SampleDeviceAppConfigViewModel : ReactiveSubscriber
  {
    private ISampleDeviceAppConfig _appConfig;

    public SampleDeviceAppConfigViewModel(ISampleDevice device, ISampleDeviceAppConfig appConfig)
    {
      _appConfig = appConfig;
      CommitValuesCommand = ReactiveCommand.Create<Unit>(t =>
     {
       device.Activate();
       AppConfig.Save();
     });
    }

    public ISampleDeviceAppConfig AppConfig
    {
      get => _appConfig;
      set => this.RaiseAndSetIfChanged(ref _appConfig, value);
    }

    public ReactiveCommand<Unit, Unit> CommitValuesCommand { get; set; }
  }
}
