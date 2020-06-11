using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using DynamicData;
using Newtonsoft.Json;
using Python.Runtime;

namespace AresAdditiveDevicesPlugin.PythonInterop.Impl
{
    public class PythonBindings : IPythonBindings
    {
        private string _amaresPath = @"..\..\..\py";
        private Dictionary<string, PyObject> _pythonClasses = new Dictionary<string, PyObject>();
        private List<PythonDef> _pythonDefs = new List<PythonDef>();
        private bool _hadAnException;
        private PyScope _pyScope;
        private readonly IPythonProcessFactory _processFactory;
        private readonly IPythonProcessConfigRepository _configRepo;
        private object _lastResult;
        private string _configPath = @"..\..\..\py\PythonProcessConfig.json";
        private bool _initComplete;

        public PythonBindings(IPythonProcessFactory processFactory, IPythonProcessConfigRepository configRepo)
        {

            _processFactory = processFactory;
            _configRepo = configRepo;
        }

        public object LastResult
        {
            get { return _lastResult; }
            set { _lastResult = value; }
        }

        public bool ConfigurationNeeded { get; set; }

        public async Task<string> Init()
        {
            try
            {
                string res = await Task.Run(() => InitPython());
                if (res.Length > 0)
                {
                    _hadAnException = true;
                    return res;
                }
            }
            catch (Exception ex)
            {
                _hadAnException = true;
                return ex.Message;
            }

            return "";
        }

        private string InitPython()
        {
            Cleanup();
            try
            {
                string pathraw = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (pathraw.Contains("x64") || pathraw.Contains("x86"))
                {
                    _amaresPath = "..\\" + _amaresPath;
                }
                string path = Path.GetFullPath(_amaresPath).Replace("\\", "/");
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
                var locker = PythonEngine.AcquireLock();
                _pyScope = Py.CreateScope("Scope");
                PythonEngine.RunSimpleString("import sys\n");
                PythonEngine.RunSimpleString("sys.path.append('" + path + "')\n");
                PythonEngine.ReleaseLock(locker);
            }
            catch (Exception fe)
            {
                return "Python configuration issue. " + fe.Message;
            }
            string[] filePaths = Directory.GetFiles(Path.GetFullPath(_amaresPath));
            List<string> fileNames = new List<string>();
            foreach (var filePath in filePaths.Where(p => p.EndsWith(".py")))
            {
                string nameWithExt = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                string nameOnly = nameWithExt.Substring(0, nameWithExt.LastIndexOf("."));
                fileNames.Add(nameOnly);
                PythonDef pd = ParseDefs(filePath, nameOnly);
                _pythonDefs.Add(pd);
            }
            try
            {
                using (Py.GIL())
                {
                    foreach (var fileName in fileNames)
                    {
                        try
                        {
                            _pythonClasses.Add(fileName, _pyScope.Import(fileName));
                        }
                        catch (Exception e)
                        {
                            _hadAnException = true;
                            return e.Message;
                        }
                    }
                }
            }
            catch (PythonException pex)
            {
                _hadAnException = true;
                return pex.Message;
            }
            if (File.Exists(_configPath))
            {
                string configString = File.ReadAllText(_configPath);
                var config = JsonConvert.DeserializeObject<PythonProcessesConfiguration>(configString);
                _configRepo.AddRange(config.ProcessConfigurations);
            }
            foreach (PythonDef def in _pythonDefs)
            {
                var processes = _processFactory.Create(this, def);
                if (processes.Any(p => !p.Configured))
                    ConfigurationNeeded = true;
            }
            _initComplete = true;
            return "";
        }

        private void Cleanup()
        {
            _pythonClasses = new Dictionary<string, PyObject>();
            _pythonDefs = new List<PythonDef>();
            _hadAnException = false;
            if (_pyScope != null)
                _pyScope.Dispose();
            _pyScope = null;
        }

        public Task<string> RunPythonDef(string className, string def, List<object> argsList)
        {
            if (argsList == null)
                argsList = new List<object>();
            if (_hadAnException)
            {
                return Task.FromResult("");
            }
            using (Py.GIL())
            {
                PyConverter converter = new PyConverter();
                PyObject[] args = new PyObject[argsList.Count];
                for (int i = 0; i < argsList.Count; i++)
                {
                    args[i] = converter.ToPython(argsList[i]);
                }
                PyTuple tuple = new PyTuple(args);

                PyObject obj = null;
                obj = _pythonClasses[className].InvokeMethod(def, tuple);
                if (obj != null)
                    LastResult = converter.ToClr(obj);
                else
                {
                    LastResult = null;
                }
            }

            return Task.FromResult("");
        }

        public async Task WriteDict(string dictName, string dictKey, double value)
        {
            var fullName = dictName + "." + dictKey;
            if (_initComplete)
                await RunPythonDef("AmaresConfig", "setConfigValue", new List<object>() { fullName, value });
        }

        public async Task WriteDict(string dictName, string dictKey, string value)
        {
            var fullName = dictName + "." + dictKey;
            if (_initComplete)
                await RunPythonDef("AmaresConfig", "setConfigValue", new List<object>() { fullName, value });
        }

        public async Task<double> GetDict(string dictName, string dictKey, int index)
        {
            var fullName = dictName + "." + dictKey;
            if (_initComplete)
                await RunPythonDef("AmaresConfig", "getConfigValue", new List<object>() { fullName, index });
            if (LastResult != null)
                return Convert.ToDouble(LastResult);
            return 0.0;
        }
        public async Task<double> GetDict(string dictName, string dictKey)
        {
            var fullName = dictName + "." + dictKey;
            if (_initComplete)
                await RunPythonDef("AmaresConfig", "getConfigValue", new List<object>() { fullName });
            if (LastResult != null)
                return Convert.ToDouble(LastResult);
            return 0.0;
        }


        private PythonDef ParseDefs(string filePath, string nameOnly)
        {
            PythonDef pd = new PythonDef();
            pd.Name = nameOnly;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("def "))
                {
                    int startpos = line.IndexOf("def") + 4;
                    string defname = line.Substring(startpos, line.LastIndexOf("(") - startpos);
                    string argslist = line.Substring(startpos + defname.Length + 1);
                    argslist = argslist.Substring(0, argslist.LastIndexOf(")"));
                    argslist = argslist.Replace(" ", "");
                    List<object> arguments = new List<object>();
                    if (argslist.Length > 0)
                    {
                        string[] allArgs = argslist.Split(',');
                        arguments = new List<object>(allArgs);
                    }
                    pd.Defs.Add(defname, arguments);
                }
            }
            file.Close();
            return pd;
        }

        public List<string> GetClassNames()
        {
            List<string> classNames = new List<string>();
            foreach (PythonDef def in _pythonDefs)
            {
                classNames.Add(def.Name);
            }
            return classNames;
        }

        public List<string> GetDefNames(string className)
        {
            foreach (PythonDef def in _pythonDefs)
            {
                if (def.Name.Equals(className))
                {
                    return new List<string>(def.Defs.Keys);
                }
            }
            return new List<string>();
        }
    }
}
