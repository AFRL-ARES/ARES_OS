using AresAdditiveDevicesPlugin.UEyeCamera.Commands;
using ARESCore.DeviceSupport;
using ARESCore.Registries;
using CommonServiceLocator;
using DynamicData.Binding;
using Newtonsoft.Json;
using Prism.Ioc;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Impl
{
  public class UEyeCameraRepo : ObservableCollectionExtended<IUEyeCamera>, IUEyeCameraRepo, IAresDevice
  {
    private int _camerasLoaded;
    private readonly CameraConfig _camConfig = JsonConvert.DeserializeObject<CameraConfig>(File.ReadAllText($@"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent?.Parent?.Parent?.Parent?.FullName}\ConfigFiles\CameraConfig.json"));

    public UEyeCameraRepo()
    {
#if !DISCONNECTED
      uEye.Info.Camera.EventNewDevice += CameraDeviceListChanged;
      uEye.Info.Camera.EventDeviceRemoved += CameraDeviceListChanged;

      CameraDeviceListChanged(this, null);
#endif
    }

    private void CameraDeviceListChanged(object sender, EventArgs eventArgs)
    {
      uEye.Info.Camera.GetCameraList(out var cameraList);
      foreach (var info in cameraList)
      {
        if (this.Any(cam => cam.SelectedCamera?.SerialNo == info.SerialNumber))
        {
          continue;
        }

        var item = new UEyeCameraProperties
        {
          CameraId = info.CameraID,
          DeviceId = info.DeviceID,
          Model = info.Model,
          SerialNo = info.SerialNumber
        };

        var camera = new UEyeCamera { SelectedCamera = item };
        if (_camConfig.ProcessCamerasSerialNumbers.Contains(item.SerialNo))
        {
          camera.SelectedCamera.CameraType = CameraType.Process;
          camera.SelectedCamera.IsAnalysisCamera = false;
        }
        if (_camConfig.AnalysisCamerasSerialNumbers.Contains(item.SerialNo))
        {
          camera.SelectedCamera.CameraType = CameraType.Analysis;
          camera.SelectedCamera.IsAnalysisCamera = true;
        }

        Add(camera);
      }
    }

    public IUEyeCamera GetCamera(int index = -1)
    {
      if (index < 0 || index >= Count)
      {
        if (CamerasLoaded < Count)
        {
          return this[CamerasLoaded++];
        }
        return this[Count - 1];
      }
      return this[index];
    }

    public Task<Bitmap> CaptureAnalysisImage()
    {
      var anlysisCamera = this.FirstOrDefault(camera => camera.SelectedCamera.IsAnalysisCamera || camera.SelectedCamera.CameraType == CameraType.Analysis);
      return anlysisCamera?.CaptureImage();
    }

    public void Dispose()
    {
      uEye.Info.Camera.EventNewDevice -= CameraDeviceListChanged;
      uEye.Info.Camera.EventDeviceRemoved -= CameraDeviceListChanged;
    }

    public int CamerasLoaded
    {
      get => _camerasLoaded;
      set
      {
        if (value != _camerasLoaded)
        {
          _camerasLoaded = value;
          OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CamerasLoaded"));
        }
      }
    }

    public void Init()
    {
    }

    public void Activate()
    {
    }

    public string Name { get; set; }
    public IAresDeviceState CurrentState { get; set; }
    public IAresDeviceState TargetState { get; set; }
    public void IssueCommand(IAresDeviceCommand command)
    {
    }

    public void RegisterCommands(IContainerRegistry registry)
    {
      var repo = ServiceLocator.Current.GetInstance<IAresCommandRegistry>();
      repo.Add(new CaptureAnalysisImageCommand());
      //      repo.Add(new UEyeCameraInitializeCommand());
      //      repo.Add(new UEyeCameraRunCameraCommand());
      //      repo.Add(new UEyeCameraSaveImageCommand());
      //      repo.Add(new UEyeCameraStopCameraCommand());
    }

    public string GetSampleScriptEntry()
    {
      return string.Empty;
    }

    public int ReadRate { get; set; }
    public bool Connected { get; }

    public string Validate()
    {
      return !this.Any() ? "Camera Repository is empty" : string.Empty;
    }
  }
}
