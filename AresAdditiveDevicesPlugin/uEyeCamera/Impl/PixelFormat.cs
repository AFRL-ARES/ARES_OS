using System;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Impl
{
    public class PixelFormat : BasicReactiveObjectDisposable, IPixelFormat
    {
        private readonly IUEyeCamera _camera;
        private int _bitWidth;
        private ColorOption _colorFormat;
        private RenderFidelity _fidelity;
        private RenderOption _renderFormat;

        protected PixelFormat(IUEyeCamera camera)
        {
            _camera = camera;
        }

        public ColorOption ColorFormat
        {
            get => _colorFormat;
            set
            {
                this.RaiseAndSetIfChanged(ref _colorFormat, value);
                Validate(1);
            }
        }

        public int BitWidth
        {
            get => _bitWidth;
            set
            {
                this.RaiseAndSetIfChanged(ref _bitWidth, value);
                Validate(2);
            }
        }

        public RenderOption RenderFormat
        {
            get => _renderFormat;
            set
            {
                this.RaiseAndSetIfChanged(ref _renderFormat, value);
                Validate(3);
            }
        }

        public RenderFidelity Fidelity
        {
            get => _fidelity;
            set
            {
                this.RaiseAndSetIfChanged(ref _fidelity, value);
                Validate(4);
            }
        }

        public virtual void StreamToCamera(IUEyeCamera camera)
        {
            throw new NotImplementedException();
        }

        protected virtual void Validate(int changedField)
        {
            throw new NotImplementedException();
        }
    }
}
