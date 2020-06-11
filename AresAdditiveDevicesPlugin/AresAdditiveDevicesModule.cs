using System;
using System.Threading.Tasks;
using AresAdditiveDevicesPlugin.Data;
using AresAdditiveDevicesPlugin.Events.Impl;
using AresAdditiveDevicesPlugin.Experiment;
using AresAdditiveDevicesPlugin.Experiment.Impl;
using AresAdditiveDevicesPlugin.Log;
using AresAdditiveDevicesPlugin.Processing;
using AresAdditiveDevicesPlugin.Processing.Components;
using AresAdditiveDevicesPlugin.Processing.Components.Base;
using AresAdditiveDevicesPlugin.Processing.Impl;
using AresAdditiveDevicesPlugin.PythonInterop;
using AresAdditiveDevicesPlugin.PythonInterop.Configuration;
using AresAdditiveDevicesPlugin.PythonInterop.Impl;
using AresAdditiveDevicesPlugin.PythonStageController;
using AresAdditiveDevicesPlugin.PythonStageController.Impl;
using AresAdditiveDevicesPlugin.PythonStageController.UI.Views;
using AresAdditiveDevicesPlugin.PythonStageController.UI.Vms;
using AresAdditiveDevicesPlugin.Terminal;
using AresAdditiveDevicesPlugin.Terminal.Views;
using AresAdditiveDevicesPlugin.UEyeCamera;
using AresAdditiveDevicesPlugin.UEyeCamera.Impl;
using AresAdditiveDevicesPlugin.UEyeCamera.Views;
using AresAdditiveDevicesPlugin.UEyeCamera.Vms;
using AresAdditiveDevicesPlugin.UI.Views;
using AresAdditiveDevicesPlugin.UI.Vms;
using ARESCore;
using ARESCore.Database.Filtering;
using ARESCore.Database.Filtering.Impl;
using ARESCore.Database.Tables;
using ARESCore.DeviceSupport;
using ARESCore.Experiment;
using ARESCore.PluginSupport;
using CommonServiceLocator;
using Ninject;
using Ninject.Extensions.Factory;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Ninject;
using Prism.Regions;

namespace AresAdditiveDevicesPlugin
{
  [Module(ModuleName = "AresAdditiveDevicesModule", OnDemand = false)]
  public class AresAdditiveDevicesModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public AresAdditiveDevicesModule(IRegionManager regionManager) : base(regionManager) { }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;


     // _containerRegistry.RegisterSingleton<IDbFilter<MachineStateEntity>, MachineStateFilter>();


      RegisterPythonModule();
      RegisterObjects();
      RegisterData();
      RegisterViews();
      RegisterRegions();
    }
    private void RegisterData()
    {
      _containerRegistry.RegisterSingleton<IDeviceScriptData, AdditiveScriptData>();
    }

    private void RegisterPythonModule()
    {

      _containerRegistry.RegisterSingleton<IPythonBindings, PythonBindings>();
      _containerRegistry.RegisterSingleton<IPythonProcessFactory, PythonProcessFactory>();
      _containerRegistry.RegisterSingleton<IPythonProcessRepository, PythonProcessRepository>();
      _containerRegistry.RegisterSingleton<IPythonProcessConfigRepository, PythonProcessConfigRepository>();
      _containerRegistry.RegisterSingleton<IConfigurationWriter, ConfigurationWriter>();
      _containerRegistry.RegisterSingleton<IPythonInvoker, PythonInvoker>();

      _containerRegistry.Register<IPythonProcess, PythonProcess>();
      _containerRegistry.Register<IComponent, PythonProcess>();
    }

    private void RegisterRegions()
    {
      var contentRegionName = "ContentRegion";
      _regionManager.RegisterViewWithRegion(contentRegionName, typeof(StatusView));

      RegisterSidebarRegion();
    }

    private void RegisterSidebarRegion()
    {
      var sidebarRegionName = "SidebarRegion";
      _regionManager.RegisterViewWithRegion(sidebarRegionName, typeof(UEyeCameraView));
      _regionManager.RegisterViewWithRegion(sidebarRegionName, typeof(UEyeCameraView));
      _regionManager.RegisterViewWithRegion(sidebarRegionName, typeof(TerminalView));
      _regionManager.RegisterViewWithRegion(sidebarRegionName, typeof(ExperimentGridView));
    }

    private void RegisterObjects()
    {
      _registry.RegisterSingleton<IToolpathParameters, ToolpathParameters>();

      _registry.Register<IUEyeCamera, UEyeCamera.Impl.UEyeCamera>();
      _registry.RegisterSingleton<IPixelFormat, UEyePixelFormat>();
      _registry.Register<UEyeCameraProperties>();
      _registry.Register<UEyeCameraPictureSettingsViewModel>();
      _registry.RegisterSingleton<ITerminal, Terminal.Impl.Terminal>();
      _registry.RegisterSingleton<ILog, Log.Impl.Log>();

      _registry.RegisterSingleton<EventHub>();
      _registry.RegisterSingleton<IComponentService, ComponentService>();
      _registry.RegisterSingleton<IPipelineService, PipelineService>();

      _registry.RegisterSingleton<ProcessPipelineSaver>();
      _registry.RegisterSingleton<ProcessPipelineLoader>();

      AresKernel._kernel.Bind<IObservable<LoadPipeLine>>().ToMethod(context => context.Kernel.Get<EventHub>().GetEvent<LoadPipeLine>()).InSingletonScope();
      AresKernel._kernel.Bind<IObservable<SavePipeLine>>().ToMethod(context => context.Kernel.Get<EventHub>().GetEvent<SavePipeLine>()).InSingletonScope();
      AresKernel._kernel.Bind<IObservable<EditStageEntry>>().ToMethod(context => context.Kernel.Get<EventHub>().GetEvent<EditStageEntry>()).InSingletonScope();
      AresKernel._kernel.Bind<IObservable<RemoveStageEntry>>().ToMethod(context => context.Kernel.Get<EventHub>().GetEvent<RemoveStageEntry>()).InSingletonScope();
      AresKernel._kernel.Bind<IObservable<OpenComponentEditor>>().ToMethod(context => context.Kernel.Get<EventHub>().GetEvent<OpenComponentEditor>()).InSingletonScope();
      AresKernel._kernel.Bind<IObservable<OpenComponentSelector>>().ToMethod(context => context.Kernel.Get<EventHub>().GetEvent<OpenComponentSelector>()).InSingletonScope();

      AresKernel._kernel.Bind<IProcessDataInputRowViewModelFactory>().ToFactory();

      _registry.RegisterSingleton<IExperimentGrid, ExperimentGrid>();
    }

    private void RegisterViews()
    {
      _containerRegistry.RegisterForNavigation<StatusView, StatusViewModel>();
      _containerRegistry.RegisterForNavigation<UEyeCameraView, UEyeCameraViewModel>();
      _containerRegistry.RegisterForNavigation<UEyeCameraFormatSettingsView, UEyeCameraFormatSettingsViewModel>();
      _containerRegistry.RegisterForNavigation<UEyeCameraPictureSettingsView, UEyeCameraPictureSettingsViewModel>();
      _containerRegistry.RegisterForNavigation<UEyeCameraSelectionView, UEyeCameraSelectionViewModel>();
      _containerRegistry.RegisterForNavigation<UEyeCameraSettingsView, UEyeCameraSettingsViewModel>();
      _containerRegistry.RegisterForNavigation<TerminalView, ITerminal>();
      _containerRegistry.RegisterForNavigation<PythonStageControllerView, PythonStageControllerViewModel>();
      _containerRegistry.RegisterForNavigation<PythonStageControllerPositionView, PythonStageControllerViewModel>();
      _containerRegistry.RegisterForNavigation<ExperimentGridView, ExperimentGridViewModel>();
      _containerRegistry.RegisterForNavigation<ToolpathConfiguratorView, ToolpathConfiguratorViewModel>();
    }

    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      ServiceLocator.Current.GetInstance<ILog>();
      var bugInfo = _containerRegistry.GetContainer();
      var componentService = ServiceLocator.Current.GetInstance<IComponentService>();

      EstablishPythonBindings();
      //      Task.Run(() => componentService.LoadOpenCvComponents());
      Task.Run(() => componentService.LoadUserDefinedComponents());
      Task.Run(() => componentService.LoadNativeComponents());

      var devices = ServiceLocator.Current.GetAllInstances<IAresDevice>();

      foreach (var device in devices)
      {
        device.Init();
        device.RegisterCommands(_registry);
      }
      foreach (var device in devices)
      {
        device.Activate();
      }
      var additive = ServiceLocator.Current.GetInstance<IDeviceScriptData>();
      var campaign = ServiceLocator.Current.GetInstance<ICampaign>();
      campaign.ExpScript = additive.ExpScript;
      campaign.InterExpScript = additive.InterExpScript;
      campaign.CampaignCloseScript = additive.CampaignCloseScript;
    }

    private async void EstablishPythonBindings()
    {
      var pythonBindings = ServiceLocator.Current.GetInstance<IPythonBindings>();
      await pythonBindings.Init();
      await ServiceLocator.Current.GetInstance<EventHub>().Publish(new TypedComponentsLoaded(typeof(PythonProcess)));

    }

  }
}
