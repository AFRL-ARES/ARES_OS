using AresAdditiveDevicesPlugin.Experiment;
using AresAdditiveDevicesPlugin.Experiment.Impl;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using AresAdditiveDevicesPlugin.PythonStageController.Impl;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.PythonStageController.UI.Vms
{
  public class
    ToolpathConfiguratorViewModel : BasicReactiveObjectDisposable
  {
    private readonly IStageController _stageController;
    private readonly PythonCommandConfig _pyConfig;
    private double _nozzleDiameter;
    private double _extrusionMultiplier;
    private double _tipHeight;
    private double _primeDistance;
    private double _primeRate;
    private double _primeDelay;
    private double _retractDistance;
    private double _retractRate;
    private double _retractDelay;
    private double _bedTemperature;
    private double _actualBedTemperature;
    private string _var1Name;
    private double _var1Value;
    private string _var2Name;
    private double _var2Value;
    private string _var3Name;
    private double _var3Value;
    private string _var4Name;
    private double _var4Value;
    private string _var5Name;
    private double _var5Value;
    private string _var6Name;
    private double _var6Value;
    private double _dispenseSpeed;
    private string _toolpathFilepath;
    private ParameterLimitConfigurations _paramterLimitsCfgs = null;

    public ToolpathConfiguratorViewModel(IStageController stageController)
    {
#if DISCONNECTED
      return;
#endif
      _paramterLimitsCfgs = JsonConvert.DeserializeObject<ParameterLimitConfigurations>(File.ReadAllText(@"..\..\..\py\ParameterLimits.json"));
      _pyConfig = File.ReadAllText(@"..\..\..\py\PythonCommandConfig.json").DeserializeObject<PythonCommandConfig>();
      _stageController = stageController;
      RunToolpathCommand = ReactiveCommand.CreateFromTask(RunToolpath);
      AbortCommand = ReactiveCommand.CreateFromTask(Abort);
      SetupConfig();
    }

    public Task SetupConfig()
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var cfgSettings = config.AppSettings.Settings;
      if (cfgSettings["Dispense Speed"] == null)
      {
        cfgSettings.Add("Dispense Speed", "0");
      }
      if (cfgSettings["Nozzle Diameter"] == null)
      {
        cfgSettings.Add("Toolpath", "");
        cfgSettings.Add("Nozzle Diameter", "0");
        cfgSettings.Add("Extrusion Multiplier", "0");
        cfgSettings.Add("Tip Height", "0");
        cfgSettings.Add("Prime Distance", "0");
        cfgSettings.Add("Prime Rate", "0");
        cfgSettings.Add("Prime Delay", "0");
        cfgSettings.Add("Retract Distance", "0");
        cfgSettings.Add("Retract Rate", "0");
        cfgSettings.Add("Retract Delay", "0");
        cfgSettings.Add("Dispense Speed", "0");
        cfgSettings.Add("Bed Temperature", "0");
        cfgSettings.Add("Var1 Name", "Var1");
        cfgSettings.Add("Var1 Value", "0");
        cfgSettings.Add("Var2 Name", "Var2");
        cfgSettings.Add("Var2 Value", "0");
        cfgSettings.Add("Var3 Name", "Var3");
        cfgSettings.Add("Var3 Value", "0");
        cfgSettings.Add("Var4 Name", "Var4");
        cfgSettings.Add("Var4 Value", "0");
        cfgSettings.Add("Var5 Name", "Var5");
        cfgSettings.Add("Var5 Value", "0");
        cfgSettings.Add("Var6 Name", "Var6");
        cfgSettings.Add("Var6 Value", "0");
      }
      ToolpathFilepath = cfgSettings["Toolpath"].Value;
      NozzleDiameter = double.Parse(cfgSettings["Nozzle Diameter"].Value);
      ExtrusionMultiplier = double.Parse(cfgSettings["Extrusion Multiplier"].Value);
      TipHeight = double.Parse(cfgSettings["Tip Height"].Value);
      PrimeDistance = double.Parse(cfgSettings["Prime Distance"].Value);
      PrimeRate = double.Parse(cfgSettings["Prime Rate"].Value);
      PrimeDelay = double.Parse(cfgSettings["Prime Delay"].Value);
      RetractDistance = double.Parse(cfgSettings["Retract Distance"].Value);
      RetractRate = double.Parse(cfgSettings["Retract Rate"].Value);
      RetractDelay = double.Parse(cfgSettings["Retract Delay"].Value);
      BedTemperature = double.Parse(cfgSettings["Bed Temperature"].Value);
      DispenseSpeed = double.Parse(cfgSettings["Dispense Speed"].Value);
      Var1Name = cfgSettings["Var1 Name"].Value;
      Var1Value = double.Parse(cfgSettings["Var1 Value"].Value);
      Var2Name = cfgSettings["Var2 Name"].Value;
      Var2Value = double.Parse(cfgSettings["Var2 Value"].Value);
      Var3Name = cfgSettings["Var3 Name"].Value;
      Var3Value = double.Parse(cfgSettings["Var3 Value"].Value);
      Var4Name = cfgSettings["Var4 Name"].Value;
      Var4Value = double.Parse(cfgSettings["Var4 Value"].Value);
      Var5Name = cfgSettings["Var5 Name"].Value;
      Var5Value = double.Parse(cfgSettings["Var5 Value"].Value);
      Var6Name = cfgSettings["Var6 Name"].Value;
      Var6Value = double.Parse(cfgSettings["Var6 Value"].Value);

      ActualBedTemperature = 0;
      return Task.CompletedTask;
    }

    private async Task RunToolpath()
    {
      if (File.Exists(ToolpathFilepath))
      {
        //        await Task.Run(async () => await _pyInvoker.InvokePythonDirect(_pyConfig.RunToolpathCommand));
        await Task.Run(_stageController.RunToolpath);
      }
    }

    private async Task Abort()
    {
      if (File.Exists(ToolpathFilepath))
      {
        //        await Task.Run(async () => await _pyInvoker.InvokePythonDirect(_pyConfig.AbortCommand));
        await Task.Run(_stageController.Abort);
      }
    }

    public Task SaveAndReload(string setting, double value)
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      if (config.AppSettings.Settings[setting] == null)
        config.AppSettings.Settings.Add(setting, value.ToString());
      config.AppSettings.Settings[setting].Value = value.ToString();
      config.Save();
      Properties.Settings.Default.Reload();


      return Task.CompletedTask;
    }

    public Task SaveAndReload(string setting, string value)
    {
      var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      if (config.AppSettings.Settings[setting] == null)
        config.AppSettings.Settings.Add(setting, value);
      config.AppSettings.Settings[setting].Value = value;
      config.Save();
      Properties.Settings.Default.Reload();
      return Task.CompletedTask;
    }

    public double NozzleDiameter
    {
      get => _nozzleDiameter;
      set
      {
        this.RaiseAndSetIfChanged(ref _nozzleDiameter, value);
        SaveAndReload("Nozzle Diameter", value);
        _stageController.WritePyDict("dispenser.diameter", value).Wait();
        _stageController.WritePyDict("dispenser.radius", Math.Round(value / 2, 6)).Wait();
        _stageController.WritePyDict("dispenser.tip_volume", Math.PI * Math.Pow(value / 2, 2)).Wait();
        ReplaceVar("dispenser.diameter", "dispenser.diameter", value);
      }
    }


    public double ExtrusionMultiplier
    {
      get => _extrusionMultiplier;
      set
      {
        this.RaiseAndSetIfChanged(ref _extrusionMultiplier, value);
        SaveAndReload("Extrusion Multiplier", value);
        _stageController.WritePyDict("dispenser.multiplier", value).Wait();
        ReplaceVar("dispenser.multiplier", "dispenser.multiplier", value);
      }
    }

    public double TipHeight
    {
      get => _tipHeight;
      set
      {
        this.RaiseAndSetIfChanged(ref _tipHeight, value);
        SaveAndReload("Tip Height", value);
        _stageController.WritePyDict("dispenser.work_dist", value).Wait();
        ReplaceVar("dispenser.work_dist", "dispenser.work_dist", value);
      }
    }

    public double PrimeDistance
    {
      get => _primeDistance;
      set
      {
        this.RaiseAndSetIfChanged(ref _primeDistance, value);
        SaveAndReload("Prime Distance", value);
        _stageController.WritePyDict("dispenser.prime", value).Wait();
        ReplaceVar("dispenser.prime", "dispenser.prime", value);

      }
    }

    public double PrimeRate
    {
      get => _primeRate;
      set
      {
        this.RaiseAndSetIfChanged(ref _primeRate, value);
        SaveAndReload("Prime Rate", value);
        _stageController.WritePyDict("dispenser.prime_rate", value).Wait();
        ReplaceVar("dispenser.prime_rate", "dispenser.prime_rate", value);

      }
    }

    public double PrimeDelay
    {
      get => _primeDelay;
      set
      {
        this.RaiseAndSetIfChanged(ref _primeDelay, value);
        SaveAndReload("Prime Delay", value);
        _stageController.WritePyDict("dispenser.prime_delay", value).Wait();
        ReplaceVar("dispenser.prime_delay", "dispenser.prime_delay", value);

      }
    }

    public double RetractDistance
    {
      get => _retractDistance;
      set
      {
        this.RaiseAndSetIfChanged(ref _retractDistance, value);
        SaveAndReload("Retract Distance", value);
        _stageController.WritePyDict("dispenser.retract", value).Wait();
        ReplaceVar("dispenser.retract", "dispenser.retract", value);
      }
    }

    public double RetractRate
    {
      get => _retractRate;
      set
      {
        this.RaiseAndSetIfChanged(ref _retractRate, value);
        SaveAndReload("Retract Rate", value);
        _stageController.WritePyDict("dispenser.retract_rate", value).Wait();
        ReplaceVar("dispenser.retract_rate", "dispenser.retract_rate", value);

      }
    }

    public double RetractDelay
    {
      get => _retractDelay;
      set
      {
        this.RaiseAndSetIfChanged(ref _retractDelay, value);
        SaveAndReload("Retract Delay", value);
        _stageController.WritePyDict("dispenser.retract_delay", value).Wait();
        ReplaceVar("dispenser.retract_delay", "dispenser.retract_delay", value);
      }
    }

    public double DispenseSpeed
    {
      get => _dispenseSpeed;
      set
      {
        this.RaiseAndSetIfChanged(ref _dispenseSpeed, value);
        SaveAndReload("Dispense Speed", value);
        _stageController.WritePyDict("dispenser.speed", value).Wait();
        ReplaceVar("dispenser.speed", "dispenser.speed", value);
      }
    }

    public double BedTemperature
    {
      get => _bedTemperature;
      set
      {
        this.RaiseAndSetIfChanged(ref _bedTemperature, value);
        SaveAndReload("Bed Temperature", value);
      }
    }

    public double ActualBedTemperature
    {
      get => _actualBedTemperature;
      set => this.RaiseAndSetIfChanged(ref _actualBedTemperature, value);
    }

    public void ReplaceVar(string varType, string varName, double varValue)
    {
      double min = 0.00001;
      double max = 10;
      varType = varType.Contains("1") ? "one" :
        varType.Contains("2") ? "two" :
          varType.Contains("3") ? "three" :
            varType.Contains("4") ? "four" :
              varType.Contains("5") ? "five" :
                varType.Contains("6") ? "six" :
                  varType;
      var paramCfg = _paramterLimitsCfgs.Limits.FirstOrDefault(param => param.Name.Equals(varName, StringComparison.CurrentCultureIgnoreCase));
      if (paramCfg != null)
      {
        min = paramCfg.Min;
        max = paramCfg.Max;
      }

      var limitConfig = _paramterLimitsCfgs.Limits.First(parameter =>
        parameter.Name.Equals(varType, StringComparison.CurrentCultureIgnoreCase));
      var varEntry = new VarEntry { Name = varType, Value = varValue, Min = limitConfig.Min, Max = limitConfig.Max };
      var toolpathParameters = ServiceLocator.Current.GetInstance<IToolpathParameters>();

      if (!toolpathParameters.ContainsKey(varType))
      {
        toolpathParameters.Add(varType, varEntry);
      }
      toolpathParameters[varType] = varEntry;
      //      ServiceLocator.Current.GetInstance<IToolpathParameters>()[varType] = new VarEntry { Value = varValue, Name = varName, Min = min, Max = max };
    }

    public string Var1Name
    {
      get => _var1Name;
      set
      {
        this.RaiseAndSetIfChanged(ref _var1Name, value);
        SaveAndReload("Var1 Name", value);
        ReplaceVar("Var1", Var1Name, Var1Value);
      }
    }

    public double Var1Value
    {
      get => _var1Value;
      set
      {
        this.RaiseAndSetIfChanged(ref _var1Value, value);
        SaveAndReload("Var1 Value", value.ToString());
        ReplaceVar("Var1", Var1Name, Var1Value);
        _stageController.WritePyDict("uservars.var1", value).Wait();
      }
    }


    public string Var2Name
    {
      get => _var2Name;
      set
      {
        this.RaiseAndSetIfChanged(ref _var2Name, value);
        SaveAndReload("Var2 Name", value);
        ReplaceVar("Var2", Var2Name, Var2Value);
      }
    }

    public double Var2Value
    {
      get => _var2Value;
      set
      {
        this.RaiseAndSetIfChanged(ref _var2Value, value);
        SaveAndReload("Var2 Value", value.ToString());
        ReplaceVar("Var2", Var2Name, Var2Value);
        _stageController.WritePyDict("uservars.var2", value).Wait();
      }
    }

    public string Var3Name
    {
      get => _var3Name;
      set
      {
        this.RaiseAndSetIfChanged(ref _var3Name, value);
        SaveAndReload("Var3 Name", value);
        ReplaceVar("Var3", Var3Name, Var3Value);
      }
    }

    public double Var3Value
    {
      get => _var3Value;
      set
      {
        this.RaiseAndSetIfChanged(ref _var3Value, value);
        SaveAndReload("Var3 Value", value.ToString());
        ReplaceVar("Var3", Var3Name, Var3Value);
        _stageController.WritePyDict("uservars.var3", value).Wait();
      }
    }


    public string Var4Name
    {
      get => _var4Name;
      set
      {
        this.RaiseAndSetIfChanged(ref _var4Name, value);
        SaveAndReload("Var4 Name", value);
        ReplaceVar("Var4", Var4Name, Var4Value);
      }
    }

    public double Var4Value
    {
      get => _var4Value;
      set
      {
        this.RaiseAndSetIfChanged(ref _var4Value, value);
        SaveAndReload("Var4 Value", value.ToString());
        ReplaceVar("Var4", Var4Name, Var4Value);
        _stageController.WritePyDict("uservars.var4", value).Wait();
      }
    }

    public string Var5Name
    {
      get => _var5Name;
      set
      {
        this.RaiseAndSetIfChanged(ref _var5Name, value);
        SaveAndReload("Var5 Name", value);
        ReplaceVar("Var5", Var5Name, Var5Value);
      }
    }

    public double Var5Value
    {
      get => _var5Value;
      set
      {
        this.RaiseAndSetIfChanged(ref _var5Value, value);
        SaveAndReload("Var5 Value", value.ToString());
        ReplaceVar("Var5", Var5Name, Var5Value);
        _stageController.WritePyDict("uservars.var5", value).Wait();
      }
    }


    public string Var6Name
    {
      get => _var6Name;
      set
      {
        this.RaiseAndSetIfChanged(ref _var6Name, value);
        SaveAndReload("Var6 Name", value);
        ReplaceVar("Var6", Var6Name, Var6Value);
      }
    }

    public double Var6Value
    {
      get => _var6Value;
      set
      {
        this.RaiseAndSetIfChanged(ref _var6Value, value);
        SaveAndReload("Var6 Value", value.ToString());
        ReplaceVar("Var6", Var6Name, Var6Value);
        _stageController.WritePyDict("uservars.var6", value).Wait();
      }
    }

    public ReactiveCommand<Unit, Unit> RunToolpathCommand { get; }
    public ReactiveCommand<Unit, Unit> AbortCommand { get; }

    public string ToolpathFilepath
    {
      get => _toolpathFilepath;
      set
      {
        this.RaiseAndSetIfChanged(ref _toolpathFilepath, value);
        SaveAndReload("Toolpath", value);
      }
    }
  }
}
