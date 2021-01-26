using ARESCore.DisposePatternHelpers;
using ARESCore.UserSession;
using MahApps.Metro.Controls;
using Ninject;
using ReactiveUI;
using System.Reactive;

namespace ARESCore.UI.ViewModels
{
  public class MainWindowViewModel : ReactiveSubscriber
  {
    private bool _deviceViewerOpen;
    private bool _deviceConfigOpen;
    private ICurrentConfig _currentConfig;
    private bool _loading;

    public MainWindowViewModel()
    {
      ShowDeviceConfigCommand = ReactiveCommand.Create<Unit, Unit>(t =>
     {
       DeviceConfigOpen = true;
       return new Unit();
     });
      HideDeviceViewerCommand = ReactiveCommand.Create<Flyout, Unit>(t =>
     {
       DeviceViewerOpen = false;
       return new Unit();
     });
      HideDeviceConfigCommand = ReactiveCommand.Create<Flyout, Unit>(t =>
     {
       DeviceConfigOpen = false;
       return new Unit();
     });
      CurrentConfig = AresKernel._kernel.Get<ICurrentConfig>();
    }

    public ReactiveCommand<Unit, Unit> ShowDeviceConfigCommand { get; set; }

    public ReactiveCommand<Flyout, Unit> HideDeviceConfigCommand { get; set; }

    public ReactiveCommand<Flyout, Unit> HideDeviceViewerCommand { get; set; }

    public bool DeviceConfigOpen
    {
      get => _deviceConfigOpen;
      set => this.RaiseAndSetIfChanged(ref _deviceConfigOpen, value);
    }
    public bool DeviceViewerOpen
    {
      get => _deviceViewerOpen;
      set => this.RaiseAndSetIfChanged(ref _deviceViewerOpen, value);
    }

    public ICurrentConfig CurrentConfig
    {
      get { return _currentConfig; }
      set { this.RaiseAndSetIfChanged(ref _currentConfig, value); }
    }

    public bool Loading
    {
      get => _loading;
      set => this.RaiseAndSetIfChanged(ref _loading, value);
    }
  }
}
