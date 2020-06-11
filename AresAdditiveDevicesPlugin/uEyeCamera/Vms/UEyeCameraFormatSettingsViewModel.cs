using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Vms
{
    public class UEyeCameraFormatSettingsViewModel : BasicReactiveObjectDisposable
    {
        private IPixelFormat _currFormat;

        public UEyeCameraFormatSettingsViewModel(IUEyeCamera camera, IPixelFormat format)
        {
            _currFormat = format;
            CommitFormatCommand = ReactiveCommand.Create(() => CurrentFormat.StreamToCamera(camera));
        }

        public IPixelFormat CurrentFormat
        {
            get => _currFormat;
            set => this.RaiseAndSetIfChanged(ref _currFormat, value);
        }
        public ReactiveCommand<Unit, Unit> CommitFormatCommand { get; set; }

        public IEnumerable<ColorOption> EnumColorOptions => Enum.GetValues(typeof(ColorOption)).Cast<ColorOption>();

        public IEnumerable<RenderOption> EnumRenderOptions => Enum.GetValues(typeof(RenderOption)).Cast<RenderOption>();

        public IEnumerable<RenderFidelity> EnumFidelityOptions => Enum.GetValues(typeof(RenderFidelity)).Cast<RenderFidelity>();

        public IEnumerable<int> EnumBitWidths => new List<int>
        {
            8,
            12,
            16,
            24,
            32
        };

        public ReactiveCommand<Unit,Unit> OkCommand { get; set; }
    }
}
