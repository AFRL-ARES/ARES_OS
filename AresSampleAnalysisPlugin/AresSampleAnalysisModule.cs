using ARESCore.AnalysisSupport;
using ARESCore.Database.Filtering;
using ARESCore.Database.Tables;
using ARESCore.PluginSupport;
using ARESCore.Registries;
using AresSampleAnalysisPlugin.Analyzers.Sample;
using AresSampleAnalysisPlugin.Analyzers.Sample.Ui.Views;
using AresSampleAnalysisPlugin.Analyzers.Sample.Ui.Vms;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AresSampleAnalysisPlugin
{
  [Module(ModuleName = "AresSampleAnalysisModule", OnDemand = false)]
  public class AresSampleAnalysisModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public AresSampleAnalysisModule(IRegionManager regionManager) : base(regionManager)
    {

    }


    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;
      _containerRegistry.RegisterSingleton<SampleAnalyzerViewModel>();
      _containerRegistry.RegisterForNavigation<SampleAnalyzerView, SampleAnalyzerViewModel>();
      containerRegistry.Register<IAresAnalyzer, SampleAnalyzer>();
    }

    private void RegisterMenuViews()
    {
      _regionManager.RegisterViewWithRegion("ContentRegion", typeof(SampleAnalyzerView));
    }

    private void RegisterResultsViews()
    {
      var resultsRegionName = "CampaignResults";
      _regionManager.RegisterViewWithRegion(resultsRegionName, typeof(SampleAnalyzerView));
    }


    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var analysisRegistry = containerProvider.Resolve<IAresAnalyzerRegistry>();
      analysisRegistry.Add(containerProvider.Resolve<SampleAnalyzer>());
      RegisterMenuViews();
      RegisterResultsViews();
    }
  }
}
