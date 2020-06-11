using ARESCore.DisposePatternHelpers;
using MahApps.Metro.Controls;
using ReactiveUI;
using System.Reactive;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Vms
{
  public class UEyeCameraSettingsViewModel : BasicReactiveObjectDisposable
  {
    private bool _isSettingsOpen;
    private bool _timingFlyoutVisible;
    private bool _formatFlyoutVisible;
    private bool _sizeFlyoutVisible;
    private bool _pictureFlyoutVisible;

    public UEyeCameraSettingsViewModel()
    {
      FormatCommand = ReactiveCommand.Create(() =>
      {
        CloseAllFlyouts();
        FormatFlyoutVisible = true;
      });
      SizeCommand = ReactiveCommand.Create(() =>
      {
        CloseAllFlyouts();
        SizeFlyoutVisible = true;
      });
      PictureCommand = ReactiveCommand.Create(() =>
      {
        CloseAllFlyouts();
        PictureFlyoutVisible = true;
      });
      TimingCommand = ReactiveCommand.Create(() =>
      {
        CloseAllFlyouts();
        TimingFlyoutVisible = true;
      });
      FormatFlyoutClosed = ReactiveCommand.Create<Flyout, bool>(flyout => FormatFlyoutVisible = false);
      SizeFlyoutClosed = ReactiveCommand.Create<Flyout, bool>(flyout => SizeFlyoutVisible = false);
      PictureFlyoutClosed = ReactiveCommand.Create<Flyout, bool>(flyout => PictureFlyoutVisible = false);
      TimingFlyoutClosed = ReactiveCommand.Create<Flyout, bool>(flyout => TimingFlyoutVisible = false);
      CloseCommand = ReactiveCommand.Create(Close);
    }

    private void Close()
    {
      SettingsOpen = false;
    }

    private void CloseAllFlyouts()
    {
      PictureFlyoutVisible = false;
      SizeFlyoutVisible = false;
      TimingFlyoutVisible = false;
      FormatFlyoutVisible = false;
    }

    public bool PictureFlyoutVisible
    {
      get => _pictureFlyoutVisible;
      set => this.RaiseAndSetIfChanged(ref _pictureFlyoutVisible, value);
    }

    public bool SizeFlyoutVisible
    {
      get => _sizeFlyoutVisible;
      set => this.RaiseAndSetIfChanged(ref _sizeFlyoutVisible, value);
    }

    public bool FormatFlyoutVisible
    {
      get => _formatFlyoutVisible;
      set => this.RaiseAndSetIfChanged(ref _formatFlyoutVisible, value);
    }

    public bool TimingFlyoutVisible
    {
      get => _timingFlyoutVisible;
      set => this.RaiseAndSetIfChanged(ref _timingFlyoutVisible, value);
    }

    public bool SettingsOpen
    {
      get => _isSettingsOpen;
      set => this.RaiseAndSetIfChanged(ref _isSettingsOpen, value);
    }

    public ReactiveCommand<Unit, Unit> FormatCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SizeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> PictureCommand { get; set; }
    public ReactiveCommand<Unit, Unit> TimingCommand { get; set; }

    public ReactiveCommand<Flyout, bool> PictureFlyoutClosed { get; set; }

    public ReactiveCommand<Flyout, bool> SizeFlyoutClosed { get; set; }

    public ReactiveCommand<Flyout, bool> TimingFlyoutClosed { get; set; }

    public ReactiveCommand<Flyout, bool> FormatFlyoutClosed { get; set; }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
  }
}
