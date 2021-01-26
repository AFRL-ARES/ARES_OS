
using System.Windows.Media;
using ARESCore.DisposePatternHelpers;

namespace ARESCore.DeviceSupport
{
  public interface ICalibrationPeak : IReactiveSubscriber
  {
    Color DefaultColor { get; }
    string Name { get; set; }
    double WaveNumber { get; set; }
    double PixelNumber { get; set; }
    Color ColorId { get; set; }
    bool DrawOnMain { get; set; }
  }
}
