using ARESCore.AnalysisSupport;
using ARESCore.PluginSupport;
using AresFCAnalysisPlugin.Analyzers;
using AresFCAnalysisPlugin.Management;
using AresFCAnalysisPlugin.Management.Impl;
using AresFCAnalysisPlugin.UI.ViewModels;
using AresFCAnalysisPlugin.UI.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AresFCAnalysisPlugin
{
  [Module(ModuleName = "AresFCAnalysisModule", OnDemand = false)]
  public class AresFCAnalysisModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public AresFCAnalysisModule(IRegionManager regionManager) : base(regionManager)
    {
    }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;
      _containerRegistry.RegisterForNavigation<AnalysisControl, AnalysisControlViewModel>();
      _containerRegistry.RegisterForNavigation<PointAnalyzerSetter, PointAnalyzerViewModel>();
      _containerRegistry.RegisterForNavigation<MaxInRangeSetter, MaxInRangeSetterViewModel>();
      _containerRegistry.RegisterSingleton<IPotentialAnalyzerRegistry, PotentialAnalyzerRegistry>();
      _containerRegistry.RegisterSingleton<IAnalyzerSetterResolver, AnalyzerSetterResolver>();
      containerRegistry.Register<IAresAnalyzer, PointAnalyzer>();
      containerRegistry.Register<IAresAnalyzer, MaxInRangeAnalyzer>();
      containerRegistry.Register<IAnalyzerSetter, PointAnalyzerSetter>();
      containerRegistry.Register<IAnalyzerSetter, MaxInRangeSetter>();
    }

    private void RegisterMenuViews()
    {
      _regionManager.RegisterViewWithRegion("ContentRegion", typeof(AnalysisControl));
    }

    private void RegisterResultsViews()
    {
      var resultsRegionName = "CampaignResults";
      // _regionManager.RegisterViewWithRegion( resultsRegionName, typeof( CampaignAnalysisResultsView ) );
    }


    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var analysisRegistry = containerProvider.Resolve<IPotentialAnalyzerRegistry>();
      analysisRegistry.Add(containerProvider.Resolve<PointAnalyzer>());
      analysisRegistry.Add(containerProvider.Resolve<MaxInRangeAnalyzer>());
      RegisterMenuViews();
      RegisterResultsViews();
    }
  }
}