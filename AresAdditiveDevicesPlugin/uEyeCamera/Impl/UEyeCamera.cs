using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ARESCore.DeviceSupport.Usb;
using ARESCore.Experiment;
using ARESCore.Experiment.Results;
using CommonServiceLocator;
using MoreLinq;
using ReactiveUI;
using uEye.Defines;
using uEye.Types;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Impl
{
  public class UEyeCamera : UsbDevice, IUEyeCamera
  {
    private const int _numberOfSeqBuffers = 3;
    private readonly DispatcherTimer _updateTimer = new DispatcherTimer();
    private bool _autoGainEnabled;
    private int _blueGain;
    private string _cameraName;
    private ulong _captureStatus;
    private BitmapImage _currentImage;
    private double _fps;
    private int _gain;
    private bool _gainBoostEnabled;
    private double _gamma;
    private bool _gammaEnabled;
    private int _greenGain;
    private bool _isAutoShutterOn;
    private bool _isAutoWhiteBalanceOn;
    private bool _isLive;
    private int _redGain;
    private readonly DisplayRenderMode _renderMode;
    private UEyeCameraProperties _selectedCamera;
    private bool _whiteBalanceOnce;
    private bool _isAssigned;
    private readonly Bitmap _capturedImage;
    private USBDeviceInfo _usbDeviceInfo;

    [System.Runtime.InteropServices.DllImport("gdi32.dll")] // because of memory leaks...
    public static extern bool DeleteObject(IntPtr hObject);

    public UEyeCamera()
    {
      Camera = new uEye.Camera();
      CurrentImage = new BitmapImage();

      _isLive = false;
      _renderMode = DisplayRenderMode.FitToWindow;

      _updateTimer.Tick += TimerTick;
      _updateTimer.Interval = new TimeSpan(0, 0, 0, 100);

#if DISCONNECTED
      Task.Run(() => Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(async _ => await TestCaptureScreen()));
#endif
    }

    private async Task TestCaptureScreen()
    {
      var bmp = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
      var graphics = Graphics.FromImage(bmp);
      graphics.CopyFromScreen(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Left, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Top, 0, 0, bmp.Size);

      await DoDrawing(bmp);
      graphics.Dispose();
      bmp.Dispose();
    }

    public uEye.Camera Camera { get; }

    public ulong CaptureStatus
    {
      get => _captureStatus;
      set => this.RaiseAndSetIfChanged(ref _captureStatus, value);
    }

    public double FPS
    {
      get => _fps;
      set => this.RaiseAndSetIfChanged(ref _fps, value);
    }

    public string CameraName
    {
      get => _cameraName;
      set => this.RaiseAndSetIfChanged(ref _cameraName, value);
    }

    public bool AutoShutter
    {
      get => _isAutoShutterOn;
      set
      {
        if (Camera.AutoFeatures.Software.Shutter.Supported)
        {
          this.RaiseAndSetIfChanged(ref _isAutoShutterOn, value);
          Camera.AutoFeatures.Software.Shutter.SetEnable(_isAutoShutterOn);
        }
      }
    }

    public BitmapImage CurrentImage
    {
      get => _currentImage;
      set => this.RaiseAndSetIfChanged(ref _currentImage, value);
    }

    public void RunCamera()
    {
      Status statusRet;
      statusRet = Initialize();

      if (statusRet == Status.SUCCESS)
      {
        // start capture
        statusRet = Camera.Acquisition.Capture();
        if (statusRet != Status.SUCCESS)
        {
          System.Windows.Forms.MessageBox.Show("Starting live video failed");
          return;
        }
        IsLive = true;
      }
      else if (Camera.IsOpened)
      {
        Camera.Exit();
      }
    }

    public void StopCamera()
    {
      Status statusRet;
      statusRet = Initialize();
      if (statusRet == Status.SUCCESS)
      {
        // start capture
        statusRet = Camera.Acquisition.Freeze();
        if (statusRet != Status.SUCCESS)
          System.Windows.Forms.MessageBox.Show("Stopping live video failed");
        else
          IsLive = false;
      }
      else if (Camera.IsOpened)
      {
        Camera.Exit();
      }
    }

    public bool AutoWhiteBalance
    {
      get => _isAutoWhiteBalanceOn;
      set
      {
        this.RaiseAndSetIfChanged(ref _isAutoWhiteBalanceOn, value);
        Camera.AutoFeatures.Software.WhiteBalance.SetEnable(_isAutoWhiteBalanceOn);
      }
    }

    public bool WhiteBalanceOnce
    {
      get => _whiteBalanceOnce;
      set
      {
        this.RaiseAndSetIfChanged(ref _whiteBalanceOnce, value);
        if (value)
          Camera.AutoFeatures.Software.WhiteBalance.SetEnable(ActivateMode.Once);
      }
    }

    public bool GammaEnabled
    {
      get => _gammaEnabled;
      set
      {
        this.RaiseAndSetIfChanged(ref _gammaEnabled, value);
        if (!value)
          Camera.Gamma.Software.Set(100);
      }
    }

    public double Gamma
    {
      get => _gamma;
      set
      {
        this.RaiseAndSetIfChanged(ref _gamma, value);
        Camera.Gamma.Software.Set((int)(_gamma * 100));
      }
    }

    public bool AutoGainEnabled
    {
      get => _autoGainEnabled;
      set
      {
        this.RaiseAndSetIfChanged(ref _autoGainEnabled, value);
        Camera.AutoFeatures.Software.Gain.SetEnable(_autoGainEnabled);
      }
    }

    public bool GainBoostEnabled
    {
      get => _gainBoostEnabled;
      set
      {
        this.RaiseAndSetIfChanged(ref _gainBoostEnabled, value);
        Camera.Gain.Hardware.Boost.SetEnable(_gainBoostEnabled);
      }
    }

    public int Gain
    {
      get => _gain;
      set
      {
        this.RaiseAndSetIfChanged(ref _gain, value);
        Camera.Gain.Hardware.Scaled.SetMaster(_gain);
      }
    }

    public void StartAcquisition()
    {
      Camera.Acquisition.Capture();
      IsLive = true;
    }

    public void StopAcquisition()
    {
      Camera.Acquisition.Stop();
      IsLive = false;
    }

    public bool IsLive
    {
      get => _isLive;
      set => this.RaiseAndSetIfChanged(ref _isLive, value);
    }

    public UEyeCameraProperties SelectedCamera
    {
      get => _selectedCamera;
      set
      {
        this.RaiseAndSetIfChanged(ref _selectedCamera, value);
        InitHardware();
      }
    }

    public bool SettingsOk
    {
      get
      {
        Camera.Information.GetSensorInfo(out var sensorInfo);
        if (sensorInfo.SensorID != Sensor.XS &&
                     sensorInfo.SensorID != Sensor.UI1008_C &&
                     sensorInfo.SensorID != Sensor.UI1013XC)
          return true;
        return false;
      }
      set { }
    }

    public void CloseCamera()
    {
      // TODO: Implement?
      //      throw new NotImplementedException();
    }

    public void CaptureVideo()
    {
      // TODO: Implement?
      //      throw new NotImplementedException();
    }

    public async Task<Bitmap> CaptureImage()
    {
#if !DISCONNECTED
      StopAcquisition();
#endif
      //      await Task.Delay( 500 );

      Bitmap bmp = null;


      using (var memStream = new MemoryStream())
      {
        BitmapFrame bmpFrame = null;
        try
        {
          // Wait for the UI thread to finish freezing and sealing the image source
          CurrentImage.Freeze(); // Just in case
          while (!CurrentImage.IsSealed || CurrentImage.StreamSource == null)
          {
            await Task.Delay(50);
          }
          var frame = BitmapFrame.Create(CurrentImage);
          frame.Freeze();
          bmpFrame = frame;
        }
        catch
        {
          return bmp;
        }
        bmpFrame.Freeze();
        var enc = new BmpBitmapEncoder();
        enc.Frames.Add(bmpFrame);
        enc.Save(memStream);

        bmp = new Bitmap(memStream);
      }
      await SaveImage(new Bitmap(bmp)); // Anywhere there is a GDI+ error, use a copy of the Bitmap
#if !DISCONNECTED
      StartAcquisition();
#endif
      return bmp;
    }

    private Task SaveImage(Image bmp)
    {
      string saveDir = string.Empty;
      var campaign = ServiceLocator.Current.GetInstance<ICampaign>();
      if (campaign.IsExecuting)
      {
        var campaignExecution = ServiceLocator.Current.GetInstance<ICampaignExecutionSummary>();
        var currentExperimentNumber = campaignExecution.ExperimentExecutionSummaries.Last().ExperimentNumber;
        var campaignsLocation =
          new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.GetDirectories().First(dir =>
            dir.Name.Equals("Campaigns", StringComparison.CurrentCultureIgnoreCase));
        var currentCampaignLocation = campaignsLocation.GetDirectories().OrderBy(dir => dir.CreationTime).Last();
        var currentExperimentLocation = currentCampaignLocation.GetDirectories()
          .First(dir => dir.Name.Equals($"Experiment_{currentExperimentNumber:000}"));
        saveDir = currentExperimentLocation.FullName;
      }
      else
      {

        var parentDir = new DirectoryInfo(@"..\..\..\Images\");
        if (!parentDir.Exists)
        {
          parentDir.Create();
        }
        saveDir = parentDir.FullName;
      }

      var timeStamp = DateTime.Now;
      var fileName = Path.Combine(saveDir, $"{timeStamp.Year:00}{timeStamp.Month:00}{timeStamp.Day:00}{timeStamp.Hour:00}{timeStamp.Minute:00}{timeStamp.Second:00}.bmp");

      using (var memory = new MemoryStream())
      {
        using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
        {
          bmp.Save(memory, ImageFormat.Jpeg);
          var buffer = memory.ToArray();
          fileStream.Write(buffer, 0, buffer.Length);
        }
      }
      return Task.CompletedTask;
    }


    public int RedGain
    {
      get => _redGain;
      set
      {
        Camera.Gain.Hardware.Scaled.SetRed(value);
        this.RaiseAndSetIfChanged(ref _redGain, value);
      }
    }

    public int GreenGain
    {
      get => _greenGain;
      set
      {
        Camera.Gain.Hardware.Scaled.SetGreen(value);
        this.RaiseAndSetIfChanged(ref _greenGain, value);
      }
    }

    public int BlueGain
    {
      get => _blueGain;
      set
      {
        Camera.Gain.Hardware.Scaled.SetBlue(value);
        this.RaiseAndSetIfChanged(ref _blueGain, value);
      }
    }

    public string ModelName
    {
      get => "UEye Camera";
      set { }
    }

    public string SerialNumber
    {
      get => _selectedCamera.SerialNo;
      set { }
    }

    public string InstrumentType
    {
      get => "Camera";
      set { }
    }

    public bool IsAssigned
    {
      get => _isAssigned;
      set => this.RaiseAndSetIfChanged(ref _isAssigned, value);
    }

    public string GetInstrumentIdentifier()
    {
      return "UEye Camera";
    }

    private void InitHardware()
    {
      Camera.Gain.Hardware.Scaled.GetMaster(out var s32Value);
      Gain = s32Value;
      Camera.Gain.Hardware.Scaled.GetRed(out s32Value);
      RedGain = s32Value;
      Camera.Gain.Hardware.Scaled.GetGreen(out s32Value);
      GreenGain = s32Value;
      Camera.Gain.Hardware.Scaled.GetBlue(out s32Value);
      BlueGain = s32Value;
      Camera.Gain.Hardware.Boost.GetEnable(out var enabled);
      GainBoostEnabled = enabled;

      Camera.AutoFeatures.Software.WhiteBalance.GetEnable(out ActivateMode activateMode);
      if (activateMode == ActivateMode.Once)
        WhiteBalanceOnce = true;
      if (activateMode == ActivateMode.Enable)
        AutoWhiteBalance = true;
    }

    private void TimerTick(object state, EventArgs eventArgs)
    {
      Camera.Timing.Framerate.GetCurrentFps(out var dFramerate);
      FPS = dFramerate;

      Camera.Information.GetCaptureStatus(out var captureStatus);
      CaptureStatus = captureStatus.Total;
    }

    private Status Initialize()
    {
      if (SelectedCamera == null)
        return Status.CANT_INIT_EVENT;
      // window handle?
      var statusRet = Camera.Init(SelectedCamera.DeviceId | (int)DeviceEnumeration.UseDeviceID);
      if (statusRet != Status.SUCCESS)
        throw new Exception("Camera Initialization Failed.");

      statusRet = MemoryHelper.AllocImageMems(Camera, _numberOfSeqBuffers);
      if (statusRet != Status.SUCCESS)
        throw new Exception("Camera Memory Allocation Failed.");

      statusRet = MemoryHelper.InitSequence(Camera);
      if (statusRet != Status.SUCCESS)
        throw new Exception("Camera Add to Sequence Failed.");

      Camera.Display.Mode.Get(out var mode);
      if (mode != DisplayMode.DiB)
        Camera.DirectRenderer.SetScaling(true);

      // set event
      Camera.EventFrame += OnFrame;

      // reset framecount

      // start update timer for our statusbar
      _updateTimer.Start();

      Camera.Information.GetSensorInfo(out SensorInfo sensorInfo);

      CameraName = sensorInfo.SensorName;
      return statusRet;
    }

    private async void OnFrame(object sender, EventArgs e)
    {
      // convert sender object to our camera object
      var camera = sender as uEye.Camera;

      if (camera.IsOpened)
      {
        camera.Display.Mode.Get(out var mode);

        // only display in dib mode
        if (mode == DisplayMode.DiB)
        {
          var statusRet = camera.Memory.GetLast(out int s32MemID);

          if (Status.SUCCESS == statusRet && 0 < s32MemID)
            if (Status.SUCCESS == camera.Memory.Lock(s32MemID))
            {
              Camera.Memory.ToBitmap(s32MemID, out var bitmap);

              if (bitmap != null && bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
              {
                await DoDrawing(bitmap);
                bitmap.Dispose();
              }
              camera.Display.Render(s32MemID, _renderMode);
              camera.Memory.Unlock(s32MemID);
            }
        }
      }
    }

    private Task DoDrawing(Bitmap bitmap)
    {
      using (var memory = new MemoryStream())
      {
        var img = new BitmapImage();

        bitmap.Save(memory, ImageFormat.Bmp);
        memory.Position = 0;
        img.BeginInit();
        img.StreamSource = memory;
        img.CacheOption = BitmapCacheOption.OnLoad;
        img.EndInit();
        img.Freeze();

        CurrentImage = img;
      }
      return Task.CompletedTask;
    }

    public override void Dispose()
    {
      Camera.EventFrame -= OnFrame;
      _updateTimer.Tick -= TimerTick;
      _updateTimer.Stop();
      IsLive = false;
      Camera.Exit();
    }

    private void OnDisplayChanged()
    {
      Camera.Display.Mode.Get(out var displayMode);

      // set scaling options
      if (displayMode != DisplayMode.DiB)
        Camera.DirectRenderer.SetScaling(true);
    }

    public override USBDeviceInfo USBDeviceInfo
    {
      get => _usbDeviceInfo;
      set => this.RaiseAndSetIfChanged(ref _usbDeviceInfo, value);
    }
  }
}
