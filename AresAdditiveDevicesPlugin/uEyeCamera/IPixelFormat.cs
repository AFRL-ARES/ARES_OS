namespace AresAdditiveDevicesPlugin.UEyeCamera
{
    public interface IPixelFormat
    {
        ColorOption ColorFormat { get; set; }
        int BitWidth { get; set; }
        RenderOption RenderFormat { get; set; }
        RenderFidelity Fidelity { get; set; }
        void StreamToCamera(IUEyeCamera camera);
    }

    public enum RenderFidelity
    {
        Normal,
        High,
        NA
    }

    public enum RenderOption
    {
        Software,
        Hardware
    }

    public enum ColorOption
    {
        Raw,
        Gray,
        RGB
    }
}