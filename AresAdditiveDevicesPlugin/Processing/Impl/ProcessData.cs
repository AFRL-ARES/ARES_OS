using System;
using AresAdditiveDevicesPlugin.Extensions;
using ARESCore.DisposePatternHelpers;
using Newtonsoft.Json;
using ReactiveUI;

namespace AresAdditiveDevicesPlugin.Processing.Impl
{
    [Serializable]
    public class ProcessData<T> : BasicReactiveObjectDisposable, IProcessData
    {
        private T _data;
        private String _name;
        private Type _type;
        private bool _requiresSetup;
        private bool _isResult;
        private bool _isAnalysisImage;

        public ProcessData(string name, T input)
        {
            Name = name;
            Data = input;
            Type = typeof(T);
        }

        public ProcessData(IProcessData source)
        {
            Name = source.Name;
            Data = source.Data;
            Type = source.Type;
            RequiresSetup = source.RequiresSetup;
            IsResult = source.IsResult;
            IsAnalysisImage = source.IsAnalysisImage;
        }

        public ProcessData(string name, Type type)
        {
            Name = name;
            Type = type;
            Data = type.FindInstantiableSubType().GenerateInstance();
        }

        [JsonProperty("Data")]
        public object Data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, (T)value);
        }

        [JsonProperty("Type")]
        public Type Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }

        [JsonProperty("Name")]
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        [JsonProperty("RequiresSetup")]
        public bool RequiresSetup
        {
            get => _requiresSetup;
            set => this.RaiseAndSetIfChanged(ref _requiresSetup, value);
        }

        [JsonProperty("IsResult")]
        public bool IsResult
        {
            get => _isResult;
            set => this.RaiseAndSetIfChanged(ref _isResult, value);
        }

        [JsonProperty("IsAnalysisImage")]
        public bool IsAnalysisImage
        {
            get => _isAnalysisImage;
            set => this.RaiseAndSetIfChanged(ref _isAnalysisImage, value);
        }

        public override string ToString()
        {
            return Name + "<" + Type?.Name + ">";
        }
    }
}
