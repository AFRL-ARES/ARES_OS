
using System.Windows.Media;
using ARESCore.DisposePatternHelpers;
using Newtonsoft.Json;
using ReactiveUI;

namespace ARESCore.DeviceSupport
{
  [JsonObject(MemberSerialization.OptOut)]
  public class CalibrationPeak : BasicReactiveObjectDisposable, ICalibrationPeak
  {
    private Color _defaultColor = Colors.Red;
    private string _name;
    private double _waveNumber;
    private double _pixelNumber;
    private Color _colorId;
    private bool _drawOnMain;

    public CalibrationPeak()
    {

    }

    public CalibrationPeak(string name, int wave, int pixel)
    {
      Name = name;
      DrawOnMain = true;
      WaveNumber = wave;
      PixelNumber = pixel;
      ColorId = DefaultColor;
    }

    [JsonConstructor]
    public CalibrationPeak(string name, double wave, double pixel, bool draw, Color colorId)
    {
      Name = name;
      DrawOnMain = draw;
      WaveNumber = wave;
      PixelNumber = pixel;
      ColorId = colorId;
    }

    public Color DefaultColor
    {
      get { return _defaultColor; }
      private set { _defaultColor = value; }
    }

    public string Name
    {
      get { return _name; }
      set { this.RaiseAndSetIfChanged(ref _name, value); }
    }

    public double WaveNumber
    {
      get { return _waveNumber; }
      set { this.RaiseAndSetIfChanged(ref _waveNumber, value); }
    }

    public double PixelNumber
    {
      get { return _pixelNumber; }
      set { this.RaiseAndSetIfChanged(ref _pixelNumber, value); }
    }

    public Color ColorId
    {
      get { return _colorId; }
      set { this.RaiseAndSetIfChanged(ref _colorId, value); }
    }

    public bool DrawOnMain
    {
      get { return _drawOnMain; }
      set { this.RaiseAndSetIfChanged(ref _drawOnMain, value); }
    }
  }
}