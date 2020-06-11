using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;

namespace AresAdditiveDevicesPlugin.PythonInterop.Impl
{
    public class PythonProcessFactory : IPythonProcessFactory
    {
        private readonly IPythonProcessRepository _repository;
        private readonly IPythonProcessConfigRepository _configRepo;
        private readonly IConfigurationWriter _configurationWriter;

        public PythonProcessFactory(IPythonProcessRepository repository, IPythonProcessConfigRepository configRepo, IConfigurationWriter configurationWriter)
        {
            _repository = repository;
            _configRepo = configRepo;
            _configurationWriter = configurationWriter;
        }
        public List<IPythonProcess> Create(IPythonBindings bindings, PythonDef definition)
        {
            List<IPythonProcess> processes = new List<IPythonProcess>();


            foreach (var def in definition.Defs)
            {
                var process = new PythonProcess(bindings, definition.Name, def, _configRepo, _configurationWriter);
                processes.Add(process);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    _repository.Add(process);
                }));
            }



            return processes;
        }
    }
}
