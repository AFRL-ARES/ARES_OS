using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
  public class ConfigurationWriter : IConfigurationWriter
  {
    private readonly string _configPath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.GetDirectories().First(dirInfo => dirInfo.Name.ToLower().StartsWith("py")).GetFiles().First(fileInfo => fileInfo.Name.ToLower().StartsWith("pythonprocessconfig")).FullName;
    public void Write(IPythonProcess process)
    {
      string processName = process.ComponentName;
      PythonProcessesConfiguration config = new PythonProcessesConfiguration();
      if (File.Exists(_configPath))
      {
        string configStringData = File.ReadAllText(_configPath);
        config = JsonConvert.DeserializeObject<PythonProcessesConfiguration>(configStringData);
      }
      PythonProcessConfiguration processConfig = null;
      if (config.ProcessConfigurations != null)
        processConfig = config.ProcessConfigurations.FirstOrDefault(n => n.ProcessName.Equals(processName));
      else
      {
        config.ProcessConfigurations = new List<PythonProcessConfiguration>();
      }
      if (processConfig != null)
        config.ProcessConfigurations.Remove(processConfig);
      var currConfig = new PythonProcessConfiguration
      {
        ProcessName = processName,
        InputConfigurations = process.InputVariables,
        OutputConfiguration = process.OutputVariable
      };
      config.ProcessConfigurations.Add(currConfig);
      string configString = JsonConvert.SerializeObject(config);
      List<string> configArray = new List<string>
      {
        configString
      };
      File.WriteAllLines(_configPath, configArray);
    }
  }
}
