using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Extensions;
using AresAdditiveDevicesPlugin.Processing.Components.BasicNative;
using AresAdditiveDevicesPlugin.Processing.Components.UserDefined;
using AresAdditiveDevicesPlugin.Processing.Impl;
using ARESCore;
using ARESCore.DisposePatternHelpers;
using CommonServiceLocator;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using Emgu.CV;
using MoreLinq;
using Newtonsoft.Json;
using Ninject.Extensions.Conventions;
using Ninject.Infrastructure.Language;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace AresAdditiveDevicesPlugin.Processing.Components.Base
{
  public class ComponentService : BasicReactiveObjectDisposable, IComponentService
  {
    private SourceList<IComponent> Components { get; } = new SourceList<IComponent>();
    private Dictionary<ComponentPairing, IObservableCollection<IProcessData>> _componentInputs = new Dictionary<ComponentPairing, IObservableCollection<IProcessData>>();
    private readonly EventHub _eventHub;

    public ComponentService(EventHub eventHub)
    {
      _eventHub = eventHub;
      AllComponents = Components.AsObservableList();
      ObservableListAlias.Select(Components.Connect(), component => component.GetType())
        .DistinctValues(componentType => componentType)
        .Bind(out var filterTypes)
        .Subscribe();

      ComponentFilterTypes = filterTypes;
    }

    public void Add(IComponent newModel)
    {
      newModel.Id = Components.Count;
      if (Components.Items.Any(component => component.ComponentName.Equals(newModel.ComponentName)))
      {
        var existing = Components.Items.First(component => component.ComponentName.Equals(newModel.ComponentName));
        Application.Current.Dispatcher.Invoke(() => Components.Remove(existing));
      }
      if (newModel is BasicNativeComponent nativeComponent)
      {
        AresKernel._kernel.Bind<BasicNativeComponent>().To(nativeComponent.GetType());
      }
      Application.Current.Dispatcher.Invoke(() => Components.Add(newModel));
    }

    public void LoadOpenCvComponents()
    {
      var cvProcessMethodInfos = typeof(CvInvoke).GetMethods();
      var openCvComponents = GenerateComponentsFromMethodInfos(cvProcessMethodInfos);
      var cvComponents = openCvComponents as IComponent[] ?? openCvComponents.ToArray();
      Application.Current.Dispatcher.Invoke(() => cvComponents.ForEach(Add));
      _eventHub.Publish(new TypedComponentsLoaded(typeof(BasicOpenCvComponent)));

    }

    public void LoadNativeComponents()
    {
      AresKernel._kernel.Bind(x =>
      {
        x.FromThisAssembly()
                .SelectAllClasses().Where(type => type.IsSubclassOf(typeof(BasicNativeComponent)))
                .BindBase();

      });
      var nativeComponents = ServiceLocator.Current.GetAllInstances<BasicNativeComponent>().ToArray();
      nativeComponents.Map(Add);
    }

    public void LoadUserDefinedComponents()
    {

      var defaultLocation = @"../../../Analyzers/";
      if (!Directory.Exists(defaultLocation))
        return;
      var jComponentFileNames = Directory.GetFiles(defaultLocation).Where(fileName => fileName.EndsWith("json"));
      foreach (var jComponentFileName in jComponentFileNames)
      {
        var jComponent = File.ReadAllText(jComponentFileName);
        var component = JsonConvert.DeserializeObject<BasicUserDefinedComponent>(jComponent);
        Add(component);
      }

      _eventHub.Publish(new TypedComponentsLoaded(typeof(BasicUserDefinedComponent)));
    }

    private IEnumerable<IComponent> GenerateComponentsFromMethodInfos(params MethodInfo[] methodInfos)
    {
      var components = new List<IComponent>();
      foreach (var method in methodInfos)
      {
        var cvParameters = method.GetParameters();
        IComponent process = null;
        try
        {
          process = GenerateComponent(method, cvParameters);

        }
        catch (Exception)
        {
          process = new BasicOpenCvComponent(method);
        }
        ((BasicOpenCvComponent)process).CvInvokeMethod = method;
        if (!components.Exists(component => component.ComponentName == process.ComponentName))
        {
          components.Add(process);
        }
      }
      return components;
    }


    private IComponent GenerateComponent(MethodInfo method, params ParameterInfo[] methodParams)
    {
      var process = new BasicOpenCvComponent(method);
      foreach (var methodParam in methodParams)
      {
        var processDataTemplateType = methodParam.ParameterType;
        if (processDataTemplateType.IsByRef || processDataTemplateType.IsPointer || processDataTemplateType.IsInstanceOfType(typeof(IInputArray)))
          continue;
        try
        {
          var templatedProcessDataType = GenerateTemplatedProcessDataType(methodParam); // ProcessData<T>
          var argName = methodParam.Name;
          var defaultArgInput = processDataTemplateType.GenerateInstance();

          if (defaultArgInput != null)
          {
            var processData = templatedProcessDataType.GenerateInstance(argName, defaultArgInput); // new ProcessData<T>(name, value)
            process.DefaultInputs.Add(processData as IProcessData);
          }
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
      }
      return process;
    }

    private Type GenerateTemplatedProcessDataType(ParameterInfo methodParam)
    {
      var processDataType = typeof(ProcessData<>);
      var processDataTemplateType = methodParam.ParameterType;
      Type templatedProcessDataType = null;
      try
      {
        templatedProcessDataType = processDataType.MakeGenericType(processDataTemplateType);
      }
      catch (Exception)
      {
        var errorAsStringType = typeof(string);
        return processDataType.MakeGenericType(errorAsStringType);
      }
      return templatedProcessDataType;

    }


    public IObservableList<IComponent> AllComponents { get; }

    public ReadOnlyObservableCollection<Type> ComponentFilterTypes { get; }

    public Dictionary<ComponentPairing, IObservableCollection<IProcessData>> ComponentInputMap
    {
      get => _componentInputs;
      set => this.RaiseAndSetIfChanged(ref _componentInputs, value);
    }
  }
}
