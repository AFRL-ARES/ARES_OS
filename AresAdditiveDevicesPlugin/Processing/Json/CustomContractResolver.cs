using System;
using System.Linq;
using AresAdditiveDevicesPlugin.Processing.Components.Base;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.Processing.Staging;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using AresAdditiveDevicesPlugin.PythonInterop.Impl;
using Newtonsoft.Json.Serialization;

namespace AresAdditiveDevicesPlugin.Processing.Json
{
    public class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            if (objectType == typeof(IProcessData) ||
                objectType.GetInterfaces().Any(inface => inface == typeof(IProcessData)))
            {
                contract.Converter = new ProcessDataJasonConverter();
            }
            else if (objectType == typeof(IStageEntry))
            {
                contract.Converter = new StageEntryJsonConverter();
            }
            else if (objectType == typeof(BasicOpenCvComponent))
            {
                contract.Converter = new BasicOpenCvComponentJsonConverter();
            }
            else if (objectType == typeof(PythonProcess))
            {
                contract.Converter = new PythonProcessJsonConverter();
            }
            else if (objectType == typeof(ProcessPipeline))
            {
                contract.Converter = new ProcessPipelineJsonConverter();
            }
            else if (objectType == typeof(BasicUserDefinedComponent))
            {
                contract.Converter = new BasicUserDefinedComponentJsonConverter();
            }
            else if (objectType == typeof(ParameterLimitConfigurations))
            {
                contract.Converter = new ParameterLimitConfigurationsConverter();
            }
            return contract;
        }
    }
}