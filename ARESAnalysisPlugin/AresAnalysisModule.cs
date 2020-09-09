using AresAnalysisPlugin.Analyzers;
using AresAnalysisPlugin.Management;
using AresAnalysisPlugin.Management.Impl;
using AresAnalysisPlugin.UI.ViewModels;
using AresAnalysisPlugin.UI.Views;
using ARESCore.AnalysisSupport;
using ARESCore.PluginSupport;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AresAnalysisPlugin
{
   [Module(ModuleName = "AresAnalysisModule", OnDemand = false)]
   public class AresAnalysisModule : AresModule
   {
      private IContainerRegistry _containerRegistry;

      public AresAnalysisModule(IRegionManager regionManager) : base(regionManager)
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