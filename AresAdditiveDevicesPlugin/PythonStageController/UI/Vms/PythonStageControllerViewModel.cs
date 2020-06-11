using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.PythonStageController.Impl;
using ARESCore.DeviceSupport;
using ARESCore.DisposePatternHelpers;
using DynamicData.Binding;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.PythonStageController.UI.Vms
{
  public class PythonStageControllerViewModel : BasicReactiveObjectDisposable
  {
    private IStageController _stageController;
    //    private readonly IPythonInvoker _pyInvoker;
    private readonly PythonCommandConfig _pyConfig;
    private int _xyFastSpeed;
    private int _xyMediumSpeed;
    private int _xySlowSpeed;
    private double _xyFastStepSize;
    private double _xyMediumStepSize;
    private double _xySlowStepSize;
    private readonly bool _fastSelected;
    private readonly bool _mediumSelected;
    private readonly bool _slowSelected;
    private int _extruderLightIntensity;
    private int _alignmentLightIntensity;
    private double _manExtruderDistance;
    private double _manExtruderRate;
    private StageControllerMotionSetting _motionSetting;
    private IObservableCollection<StageControllerMotionSetting> _motionSettings = new ObservableCollectionExtended<StageControllerMotionSetting> { StageControllerMotionSetting.Slow, StageControllerMotionSetting.Medium, StageControllerMotionSetting.Fast };

    public PythonStageControllerViewModel(IAresDevice[] devices)
    {
      _pyConfig = File.ReadAllText(@"..\..\..\py\PythonCommandConfig.json").DeserializeObject<PythonCommandConfig>();
      //      StageController = stageController;
      StageController = (IStageController)devices
        .First(device => device is IStageController);
      SetupConfig();
      YPlusCommand = ReactiveCommand.CreateFromTask(YPlus);
      YMinusCommand = ReactiveCommand.CreateFromTask(YMinus);
      XPlusCommand = ReactiveCommand.CreateFromTask(XPlus);
      XMinusCommand = ReactiveCommand.CreateFromTask(XMinus);
      UpLeftCommand = ReactiveCommand.CreateFromTask(UpLeft);
      UpRightCommand = ReactiveCommand.CreateFromTask(UpRight);
      DownLeftCommand = ReactiveCommand.CreateFromTask(DownLeft);
      DownRightCommand = ReactiveCommand.CreateFromTask(DownRight);
      ExtruderToolSelectCommand = ReactiveCommand.CreateFromTask(SelectExtruderTool);
      AlignmentToolSelectCommand = ReactiveCommand.CreateFromTask(SelectAlignmentTool);
      ZPlusCommand = ReactiveCommand.CreateFromTask(ZPlus);
      ZMinusCommand = ReactiveCommand.CreateFromTask(ZMinus);
      BedLevelCommand = ReactiveCommand.CreateFromTask(StageLevel);
      HomeCommand = ReactiveCommand.CreateFromTask(Home);
      SetHomeCommand = ReactiveCommand.CreateFromTask(SetHome);
      MaintenancePositionCommand = ReactiveCommand.CreateFromTask(MaintPos);
      EStopCommand = ReactiveCommand.CreateFromTask(EStop);
      RemoveSampleCommand = ReactiveCommand.CreateFromTask(RemoveSample);
      ChangeTipCommand = ReactiveCommand.CreateFromTask(ChangeTip);
      OffsetDefintionCommand = ReactiveCommand.CreateFromTask(OffsetDefinition);
      GenerateToolpathCommand = ReactiveCommand.CreateFromTask(GenerateToolpath);
      GoHomeCommand = ReactiveCommand.CreateFromTask(GoHome);
      // TODO: See the Prime() and Retract() methods further below
      ManualPrime = ReactiveCommand.CreateFromTask(Prime);
      ManualRetract = ReactiveCommand.CreateFromTask(Retract);
      CleanTipCommand = ReactiveCommand.CreateFromTask(CleanTip);

      /*      if (!string.IsNullOrEmpty(StageController.ErrorText))
            {
              ErrorText = StageController.ErrorText;
              ErrorVisible = true;
            }*/
      MotionSetting = StageControllerMotionSetting.Slow;
    }

    private async Task SelectAlignmentTool()
    {
      await _stageController.SelectAlignmentTool();
    }

    private async Task SelectExtruderTool()
    {
      await _stageController.SelectExtruderTool();
    }

    private void SetupConfig()
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var settings = config.AppSettings.Settings;
      if (settings["Extruder Light Intensity"] == null)
      {
        settings.Add("Extruder Light Intensity", "0");
        settings.Add("Alignment Light Intensity", "0");
        config.Save();
        Properties.Settings.Default.Reload();
      }
      if (settings["Slow Step"] == null)
      {
        settings.Add("Slow Step", "0");
        settings.Add("Medium Step", "0");
        settings.Add("Fast Step", "0");
        settings.Add("Slow Speed", "0");
        settings.Add("Medium Speed", "0");
        settings.Add("Fast Speed", "0");
        config.Save();
        Properties.Settings.Default.Reload();
      }
      if (settings["Manual Extrude Distance"] == null)
      {
        settings.Add("Manual Extrude Distance", "0.1");
        settings.Add("Manual Extrude Rate", "50");
        config.Save();
        Properties.Settings.Default.Reload();
      }
      ExtruderLightIntensity = int.Parse(settings["Extruder Light Intensity"].Value);
      AlignmentLightIntensity = int.Parse(settings["Alignment Light Intensity"].Value);


      XYSlowStepSize = double.Parse(settings["Slow Step"].Value);
      XYMediumStepSize = double.Parse(settings["Medium Step"].Value);
      XYFastStepSize = double.Parse(settings["Fast Step"].Value);
      XYSlowSpeed = int.Parse(settings["Slow Speed"].Value);
      XYMediumSpeed = int.Parse(settings["Medium Speed"].Value);
      XYFastSpeed = int.Parse(settings["Fast Speed"].Value);

      ExtruderDistance = double.Parse(settings["Manual Extrude Distance"].Value);
      ExtruderRate = double.Parse(settings["Manual Extrude Rate"].Value);
      config = null;

    }

    private async Task GenerateToolpath()
    {
      await StageController.GenerateToolpath();
    }

    private async Task OffsetDefinition()
    {
      await StageController.OffsetDefinition();
    }

    private async Task ChangeTip()
    {
      await StageController.ChangeTip();
    }

    private async Task CleanTip()
    {
      await StageController.CleanTip();
    }

    private async Task RemoveSample()
    {
      await StageController.RemoveSample();
    }

    private async Task EStop()
    {
      await StageController.EStop();
    }

    private async Task MaintPos()
    {
      await StageController.MaintenancePosition();
    }

    private async Task Home()
    {
      await _stageController.Home();
    }

    private async Task SetHome()
    {
      await _stageController.SetHome();
    }

    private async Task StageLevel()
    {
      await _stageController.Level();
    }

    public async Task GoHome()
    {
      await _stageController.GoHome();
    }

    private async Task ZMinus()
    {
      await _stageController.StepZ(-_stageController.ZStepSizeSetting);
    }

    private async Task ZPlus()
    {
      await _stageController.StepZ(_stageController.ZStepSizeSetting);
    }

    private async Task XMinus()
    {
      await _stageController.StepX(-_stageController.XYStepSizeSetting);
    }

    private async Task XPlus()
    {
      await _stageController.StepX(_stageController.XYStepSizeSetting);
    }

    private async Task YPlus()
    {
      await _stageController.StepY(_stageController.XYStepSizeSetting);
    }

    private async Task YMinus()
    {
      await _stageController.StepY(-_stageController.XYStepSizeSetting);

    }

    private async Task UpLeft()
    {
      await StageController.StepNorthWest(StageController.XYStepSizeSetting);

    }

    private async Task UpRight()
    {
      await StageController.StepNorthEast(StageController.XYStepSizeSetting);
    }

    private async Task DownLeft()
    {
      await StageController.StepSouthWest(-StageController.XYStepSizeSetting);

    }

    private async Task DownRight()
    {
      await StageController.StepSouthEast(-StageController.XYStepSizeSetting);
    }

    // TODO: Add these tasks to the python stage controller class
    private async Task Prime()
    {
      await StageController.Prime(ExtruderDistance, ExtruderRate);
    }

    private async Task Retract()
    {
      await StageController.Retract(ExtruderDistance, ExtruderRate);
    }

    public ReactiveCommand<Unit, Unit> YPlusCommand { get; set; }
    public ReactiveCommand<Unit, Unit> YMinusCommand { get; set; }
    public ReactiveCommand<Unit, Unit> XPlusCommand { get; set; }
    public ReactiveCommand<Unit, Unit> XMinusCommand { get; set; }
    public ReactiveCommand<Unit, Unit> UpLeftCommand { get; set; }
    public ReactiveCommand<Unit, Unit> UpRightCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DownLeftCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DownRightCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ZPlusCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ZMinusCommand { get; set; }
    public ReactiveCommand<Unit, Unit> BedLevelCommand { get; set; }
    public ReactiveCommand<Unit, Unit> HomeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SetHomeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveSampleCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OffsetDefintionCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeTipCommand { get; set; }
    public ReactiveCommand<Unit, Unit> GenerateToolpathCommand { get; set; }
    public ReactiveCommand<Unit, Unit> GoHomeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ExtruderToolSelectCommand { get; set; }
    public ReactiveCommand<Unit, Unit> AlignmentToolSelectCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ManualPrime { get; set; }
    public ReactiveCommand<Unit, Unit> ManualRetract { get; set; }
    public ReactiveCommand<Unit, Unit> CleanTipCommand { get; set; }

    public IStageController StageController
    {
      get => _stageController;
      set => this.RaiseAndSetIfChanged(ref _stageController, value);
    }

    public ReactiveCommand<Unit, Unit> MaintenancePositionCommand { get; set; }
    public ReactiveCommand<Unit, Unit> EStopCommand { get; set; }

    public double XYSlowStepSize
    {
      get => _xySlowStepSize;
      set
      {
        if (value != _xySlowStepSize)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Slow Step"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;
        }
        this.RaiseAndSetIfChanged(ref _xySlowStepSize, value);
        if (MotionSetting == StageControllerMotionSetting.Slow)
        {
          StageController.XYStepSizeSetting = value;
        }
      }
    }

    public double XYMediumStepSize
    {
      get => _xyMediumStepSize;
      set
      {
        if (value != _xyMediumStepSize)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Medium Step"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;

        }
        this.RaiseAndSetIfChanged(ref _xyMediumStepSize, value);
        if (MotionSetting == StageControllerMotionSetting.Medium)
        {
          StageController.XYStepSizeSetting = value;
        }
      }
    }

    public double XYFastStepSize
    {
      get => _xyFastStepSize;
      set
      {
        if (value != _xyFastStepSize)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Fast Step"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;

        }
        this.RaiseAndSetIfChanged(ref _xyFastStepSize, value);
        if (MotionSetting == StageControllerMotionSetting.Fast)
        {
          StageController.XYStepSizeSetting = value;
        }
      }
    }

    public int XYSlowSpeed
    {
      get => _xySlowSpeed;
      set
      {
        if (value != _xySlowSpeed)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Slow Speed"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;

        }
        this.RaiseAndSetIfChanged(ref _xySlowSpeed, value);
        if (MotionSetting == StageControllerMotionSetting.Slow)
        {
          StageController.XYSpeedSetting = value;
        }
      }
    }

    public int XYMediumSpeed
    {
      get => _xyMediumSpeed;
      set
      {
        if (value != _xyMediumSpeed)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Medium Speed"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;

        }
        this.RaiseAndSetIfChanged(ref _xyMediumSpeed, value);
        if (MotionSetting == StageControllerMotionSetting.Medium)
        {
          StageController.XYSpeedSetting = value;
        }
      }
    }

    public int XYFastSpeed
    {
      get => _xyFastSpeed;
      set
      {
        if (value != _xyFastSpeed)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Fast Speed"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;

        }
        this.RaiseAndSetIfChanged(ref _xyFastSpeed, value);
        if (MotionSetting == StageControllerMotionSetting.Fast)
        {
          StageController.XYSpeedSetting = value;
        }
      }
    }

    public StageControllerMotionSetting MotionSetting
    {
      get => _motionSetting;
      set
      {
        this.RaiseAndSetIfChanged(ref _motionSetting, value);
        if (value == StageControllerMotionSetting.Slow)
        {
          StageController.XYStepSizeSetting = XYSlowStepSize;
          StageController.XYSpeedSetting = XYSlowSpeed;

        }
        else if (value == StageControllerMotionSetting.Medium)
        {
          StageController.XYStepSizeSetting = XYMediumStepSize;
          StageController.XYSpeedSetting = XYMediumSpeed;
        }
        else if (value == StageControllerMotionSetting.Fast)
        {
          StageController.XYStepSizeSetting = XYFastStepSize;
          StageController.XYSpeedSetting = XYFastSpeed;
        }
      }
    }

    public IObservableCollection<StageControllerMotionSetting> MotionSettings
    {
      get => _motionSettings;
      set => this.RaiseAndSetIfChanged(ref _motionSettings, value);
    }

    //    public bool SlowSelected
    //    {
    //      get => _slowSelected;
    //      set
    //      {
    //        this.RaiseAndSetIfChanged(ref _slowSelected, value);
    //        if (value)
    //        {
    //          StageController.XYStepSizeSetting = XYSlowStepSize;
    //          StageController.XYSpeedSetting = XYSlowSpeed;
    //        }
    //      }
    //    }
    //
    //    public bool MediumSelected
    //    {
    //      get => _mediumSelected;
    //      set
    //      {
    //        this.RaiseAndSetIfChanged(ref _mediumSelected, value);
    //        if (value)
    //        {
    //          StageController.XYStepSizeSetting = XYMediumStepSize;
    //          StageController.XYSpeedSetting = XYMediumSpeed;
    //        }
    //      }
    //    }
    //
    //    public bool FastSelected
    //    {
    //      get => _fastSelected;
    //      set
    //      {
    //        this.RaiseAndSetIfChanged(ref _fastSelected, value);
    //        if (value)
    //        {
    //          StageController.XYStepSizeSetting = XYFastStepSize;
    //          StageController.XYSpeedSetting = XYFastSpeed;
    //        }
    //      }
    //    }

    public int ExtruderLightIntensity
    {
      get => _extruderLightIntensity;
      set
      {
        if (value != _extruderLightIntensity)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Extruder Light Intensity"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;

        }
        this.RaiseAndSetIfChanged(ref _extruderLightIntensity, value);
        StageController.ExtruderLightIntensity = value;
      }
    }

    public int AlignmentLightIntensity
    {
      get => _alignmentLightIntensity;
      set
      {
        if (value != _alignmentLightIntensity)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Alignment Light Intensity"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;

        }
        this.RaiseAndSetIfChanged(ref _alignmentLightIntensity, value);
        StageController.AlignmentLightIntensity = value;
      }
    }

    public double ExtruderDistance
    {
      get => _manExtruderDistance;
      set
      {
        if (value != _manExtruderDistance)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Manual Extrude Distance"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;
        }
        this.RaiseAndSetIfChanged(ref _manExtruderDistance, value);
      }
    }

    public double ExtruderRate
    {
      get => _manExtruderRate;
      set
      {
        if (value != _manExtruderRate)
        {
          var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          config.AppSettings.Settings["Manual Extrude Rate"].Value = value.ToString();
          config.Save(ConfigurationSaveMode.Modified);
          Properties.Settings.Default.Reload();
          config = null;
        }
        this.RaiseAndSetIfChanged(ref _manExtruderRate, value);
      }
    }
  }
}
