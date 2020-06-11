using uEye.Defines;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Impl
{
    public class UEyePixelFormat : PixelFormat
    {
        private ColorMode _colorMode = 0;
        private ColorConvertMode _convertMode = 0;

        public UEyePixelFormat(IUEyeCamera camera) : base(camera)
        {
            ColorFormat = ColorOption.Gray;
            BitWidth = 8;
            RenderFormat = RenderOption.Software;
            Fidelity = RenderFidelity.Normal;
            //InitializeValues( camera );
        }

        private void InitializeValues(IUEyeCamera camera)
        {
            if (!(camera is UEyeCamera))
                return;
            var uEyeCamera = camera as UEyeCamera;
            ColorMode colormode;
            uEyeCamera.Camera.PixelFormat.Get(out colormode);
            ColorConvertMode convertmode;
            uEyeCamera.Camera.Color.Converter.Get(colormode, out convertmode);
            switch (convertmode)
            {
                case ColorConvertMode.Hardware3X3:
                    RenderFormat = RenderOption.Hardware;
                    break;
                case ColorConvertMode.Software3X3:
                    RenderFormat = RenderOption.Software;
                    Fidelity = RenderFidelity.Normal;
                    break;
                case ColorConvertMode.Software5X5:
                    RenderFormat = RenderOption.Software;
                    Fidelity = RenderFidelity.High;
                    break;
                default:
                    RenderFormat = RenderOption.Software;
                    Fidelity = RenderFidelity.Normal;
                    break;
            }
            switch (colormode)
            {
                case ColorMode.SensorRaw8:
                    BitWidth = 8;
                    ColorFormat = ColorOption.Raw;
                    break;
                case ColorMode.SensorRaw12:
                    BitWidth = 12;
                    ColorFormat = ColorOption.Raw;
                    break;
                case ColorMode.SensorRaw16:
                    BitWidth = 16;
                    ColorFormat = ColorOption.Raw;
                    break;
                case ColorMode.Mono8:
                    BitWidth = 8;
                    ColorFormat = ColorOption.Gray;
                    break;
                case ColorMode.Mono12:
                    BitWidth = 12;
                    ColorFormat = ColorOption.Gray;
                    break;
                case ColorMode.Mono16:
                    BitWidth = 16;
                    ColorFormat = ColorOption.Gray;
                    break;
                case ColorMode.BGR8Packed:
                    BitWidth = 24;
                    ColorFormat = ColorOption.RGB;
                    break;
                case ColorMode.BGRA8Packed:
                    BitWidth = 32;
                    ColorFormat = ColorOption.RGB;
                    break;
                default:
                    BitWidth = 8;
                    ColorFormat = ColorOption.Raw;
                    break;
            }
        }

        public override void StreamToCamera(IUEyeCamera camera)
        {
            if (!(camera is UEyeCamera)) return;

            var uEyeCamera = camera as UEyeCamera;


            if (ColorFormat == ColorOption.Raw)
                HandleRaw();
            if (ColorFormat == ColorOption.Gray)
                HandleGray();
            if (ColorFormat == ColorOption.RGB)
                HandleRGB();
            HandleConvert();

            bool isLive;
            uEyeCamera.Camera.Acquisition.HasStarted(out isLive);

            if (isLive)
                uEyeCamera.Camera.Acquisition.Stop();

            uEyeCamera.Camera.PixelFormat.Set(_colorMode);
            uEyeCamera.Camera.Color.Converter.Set(_colorMode, _convertMode);

            // memory reallocation
            int[] idList;
            uEyeCamera.Camera.Memory.GetList(out idList);

            MemoryHelper.ClearSequence(uEyeCamera.Camera);
            MemoryHelper.FreeImageMems(uEyeCamera.Camera);

            MemoryHelper.AllocImageMems(uEyeCamera.Camera, idList.Length);
            MemoryHelper.InitSequence(uEyeCamera.Camera);

            if (isLive)
                uEyeCamera.Camera.Acquisition.Capture();
        }

        private void HandleConvert()
        {
            if (RenderFormat == RenderOption.Software)
                if (Fidelity == RenderFidelity.Normal)
                    _convertMode = ColorConvertMode.Software3X3;
                else
                    _convertMode = ColorConvertMode.Software5X5;
            else
                _convertMode = ColorConvertMode.Hardware3X3;
        }

        private void HandleRGB()
        {
            if (BitWidth == 24)
                _colorMode = ColorMode.BGR8Packed;
            if (BitWidth == 32)
                _colorMode = ColorMode.BGRA8Packed;
        }

        private void HandleGray()
        {
            if (BitWidth == 8)
                _colorMode = ColorMode.Mono8;
            if (BitWidth == 10)
                _colorMode = ColorMode.Mono10;
            if (BitWidth == 12)
                _colorMode = ColorMode.Mono12;
            if (BitWidth == 16)
                _colorMode = ColorMode.Mono16;
        }

        private void HandleRaw()
        {
            if (BitWidth == 8)
                _colorMode = ColorMode.SensorRaw8;
            if (BitWidth == 12)
                _colorMode = ColorMode.SensorRaw12;
            if (BitWidth == 16)
                _colorMode = ColorMode.SensorRaw16;
        }

        protected override void Validate(int changedField)
        {
            if (changedField == 1)
                if ((ColorFormat == ColorOption.Gray || ColorFormat == ColorOption.Raw) && BitWidth > 16)
                    BitWidth = 16;
                else if (ColorFormat == ColorOption.RGB)
                    if (BitWidth < 24)
                        BitWidth = 24;
            if (changedField == 2)
                if (BitWidth > 16 && (ColorFormat == ColorOption.Gray || ColorFormat == ColorOption.Raw))
                    ColorFormat = ColorOption.RGB;
                else if (BitWidth < 24 && ColorFormat == ColorOption.RGB)
                    ColorFormat = ColorOption.Gray;
            if (changedField == 3)
                if (RenderFormat == RenderOption.Software && Fidelity == RenderFidelity.NA)
                    Fidelity = RenderFidelity.Normal;
                else if (RenderFormat == RenderOption.Hardware && (Fidelity == RenderFidelity.High || Fidelity == RenderFidelity.Normal))
                    Fidelity = RenderFidelity.NA;
            if (changedField == 4)
                if (RenderFormat == RenderOption.Software && Fidelity == RenderFidelity.NA)
                    RenderFormat = RenderOption.Hardware;
                else if (RenderFormat == RenderOption.Hardware && (Fidelity == RenderFidelity.High || Fidelity == RenderFidelity.Normal))
                    RenderFormat = RenderOption.Software;
        }
    }
}
