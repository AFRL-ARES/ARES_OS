using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;
using Newtonsoft.Json;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.PythonInterop.Configuration
{
    public class PythonVariableConfiguration : BasicReactiveObjectDisposable
    {
        private int _selectedVariableType;
        private Type _variableType;
        private string _variableName;

        public PythonVariableConfiguration()
        {
            SelectedVariableType = 0;
        }

        [JsonProperty("VariableName")]
        public string VariableName
        {
            get { return _variableName; }
            set { this.RaiseAndSetIfChanged(ref _variableName, value); }
        }
        [JsonProperty("VariableType")]
        public Type VariableType
        {
            get { return _variableType; }
            set
            {
                bool shouldUpdate = _variableType != value;
                this.RaiseAndSetIfChanged(ref _variableType, value);
                if (shouldUpdate)
                    SetSelectedVariableType(value);

            }
        }

        private void SetSelectedVariableType(Type value)
        {
            if (value == typeof(int))
                SelectedVariableType = 0;
            else if (value == typeof(float))
                SelectedVariableType = 1;
            else if (value == typeof(double))
                SelectedVariableType = 2;
            else if (value == typeof(string))
                SelectedVariableType = 3;
            else
            {
                SelectedVariableType = 4;
            }
        }

        public string[] ValidObjectTypes => new[] { "Integer", "Float", "Double", "Text", "Unknown" };

        public int SelectedVariableType
        {
            get { return _selectedVariableType; }
            set
            {
                bool shouldUpdate = _selectedVariableType != value;
                this.RaiseAndSetIfChanged(ref _selectedVariableType, value);
                if (shouldUpdate)
                    SetVariableType(value);
            }

        }

        private void SetVariableType(int value)
        {
            switch (value)
            {
                case 0:
                    VariableType = typeof(int);
                    break;
                case 1:
                    VariableType = typeof(float);
                    break;
                case 2:
                    VariableType = typeof(double);
                    break;
                case 3:
                    VariableType = typeof(string);
                    break;
                default:
                    VariableType = typeof(object);
                    break;
            }
        }
    }
}
