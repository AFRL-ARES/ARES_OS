using AresAdditiveAnalysisPlugin.Analyzers;
using AresAdditiveAnalysisPlugin.Database.Filtering;
using AresAdditiveAnalysisPlugin.Database.Filtering.Impl;
using AresAdditiveAnalysisPlugin.Database.UI;
using AresAdditiveAnalysisPlugin.Database.UI.ViewModels;
using AresAdditiveAnalysisPlugin.UI.ViewModels;
using AresAdditiveAnalysisPlugin.UI.Views;
using ARESCore.Database.Filtering;
using ARESCore.Database.Tables;
using ARESCore.PluginSupport;
using ARESCore.Registries;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AresAdditiveAnalysisPlugin
{
  [Module(ModuleName = "AresAdditiveAnalysisModule", OnDemand = false)]
  public class AresAdditiveAnalysisModule : AresModule
  {
    private IContainerRegistry _containerRegistry;

    public AresAdditiveAnalysisModule(IRegionManager regionManager) : base(regionManager)
    {
    }

    public override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      base.RegisterTypes(containerRegistry);
      _containerRegistry = containerRegistry;

      RegisterSingletons();
      RegisterTransients();
      RegisterViews();
      RegisterRegions();
    }

    private void RegisterSingletons()
    {
      _containerRegistry.RegisterSingleton<IDataFilterOptions, DataFilterOptions>();
      _containerRegistry.RegisterSingleton<IDbFilter<DataEntity>, DataFilter>();
    }

    private void RegisterTransients()
    {
      //            _containerRegistry.Register<>();
    }

    private void RegisterViews()
    {
      _containerRegistry.RegisterForNavigation<AdditiveDbFilterView, AdditiveDbFilterViewModel>();
      _containerRegistry.RegisterForNavigation<AdditiveImageAnalysisView, AdditiveImageAnalysisViewModel>();
    }

    private void RegisterRegions()
    {
      var regionName = "CampaignResults";
      _regionManager.RegisterViewWithRegion(regionName, typeof(AdditiveImageAnalysisView));
    }

    public override void OnInitialized(IContainerProvider containerProvider)
    {
      base.OnInitialized(containerProvider);
      var analysisRegistry = containerProvider.Resolve<IAresAnalyzerRegistry>();
      var blueLineAnalyzer = containerProvider.Resolve<AdditiveBlueLineStraightnessAnalyzer>();
      var whiteOnBlackAnalyzer = containerProvider.Resolve<AdditiveWhiteOnBlackAnalyzer>();
      analysisRegistry.Add(blueLineAnalyzer);
      analysisRegistry.Add(whiteOnBlackAnalyzer);
    }
  }
}
