using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.PythonInterop.Impl
{
    public class PythonDef : BasicReactiveObjectDisposable
    {
        private string _className;
        private Dictionary<string, List<object>> _defs = new Dictionary<string, List<object>>();
        private bool _isAsset;

        public string Name
        {
            get => _className;
            set => this.RaiseAndSetIfChanged(ref _className, value);
        }
        public Dictionary<string, List<object>> Defs
        {
            get => _defs;
        }

        public bool IsAsset
        {
            get => _isAsset;
            set => this.RaiseAndSetIfChanged(ref _isAsset, value);
        }
        public void Add(string defname, List<object> arguments)
        {
            _defs.Add(defname, arguments);
            this.RaisePropertyChanged("Defs");
        }
    }
}
