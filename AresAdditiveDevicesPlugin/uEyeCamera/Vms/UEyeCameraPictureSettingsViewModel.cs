using System.Reactive;
using ARESCore.DisposePatternHelpers;
using MahApps.Metro.Controls;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Vms
{
    public class UEyeCameraPictureSettingsViewModel : BasicReactiveObjectDisposable
    {
        private IUEyeCamera _camera;

        public UEyeCameraPictureSettingsViewModel(IUEyeCamera camera)
        {
            _camera = camera;

            WhiteBalanceCommand = ReactiveCommand.Create<ToggleSwitch>(t =>
            {
                if (t.IsChecked != null && t.IsChecked.Value)
                    Camera.WhiteBalanceOnce = false;
            });
            WhiteBalanceOnceCommand = ReactiveCommand.Create<ToggleSwitch>(t =>
            {
                if (t.IsChecked != null && t.IsChecked.Value)
                    Camera.AutoWhiteBalance = false;
            });
            GammaCommand = ReactiveCommand.Create<ToggleSwitch>(t =>
            {
                if (t.IsChecked != null) Camera.GammaEnabled = t.IsChecked.Value;
            });
        }

        public IUEyeCamera Camera
        {
            get => _camera;
            set => this.RaiseAndSetIfChanged(ref _camera, value);
        }

        public ReactiveCommand<ToggleSwitch, Unit> WhiteBalanceCommand { get; set; }

        public ReactiveCommand<ToggleSwitch, Unit> WhiteBalanceOnceCommand { get; set; }

        public ReactiveCommand<ToggleSwitch, Unit> GammaCommand { get; set; }
    }
}
