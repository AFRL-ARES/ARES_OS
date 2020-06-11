using ARESCore.Commands;
using ARESCore.Configurations;
using ARESCore.Configurations.impl;
using ARESCore.Database.Filtering;
using ARESCore.Database.Filtering.Impl;
using ARESCore.Database.Management;
using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;
using ARESCore.DataHub;
using ARESCore.ErrorSupport;
using ARESCore.ErrorSupport.Impl;
using ARESCore.ErrorSupport.UI;
using ARESCore.Experiment;
using ARESCore.Experiment.impl;
using ARESCore.Experiment.Results;
using ARESCore.Experiment.Results.impl;
using ARESCore.Experiment.Scripting;
using ARESCore.Experiment.Scripting.Commands;
using ARESCore.Experiment.Scripting.Impl;
using ARESCore.Experiment.UI.ViewModels;
using ARESCore.PlanningSupport;
using ARESCore.PlanningSupport.Impl;
using ARESCore.Registries;
using ARESCore.Registries.Impl;
using ARESCore.Resources;
using ARESCore.Starter;
using ARESCore.UI;
using ARESCore.UI.ViewModels;
using ARESCore.UserSession;
using ARESCore.UserSession.Impl;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace ARESCore
{
  public class ARESCoreModule : NinjectModule
  {
    public override void Load()
    {
      Bind<IErroredObjectRepo>().To<ErroredObjectRepo>().InSingletonScope();
      Bind<CampaignExecutionViewModel>().ToSelf().InSingletonScope();

      Bind<IExperimentExecutionSummary>().To<ExperimentExecutionSummary>().InTransientScope();
      Bind<IStepExecutionSummary>().To<StepExecutionExecutionSummary>().InTransientScope();
      Bind<ICommandExecutionSummary>().To<CommandExecutionSummary>().InTransientScope();
      Bind<ICampaignExecutionSummary>().To<CampaignExecutionSummary>().InSingletonScope();

      Bind<IAresStarter>().To<AresStarter>().InSingletonScope();
      Bind<IResourceManager>().To<ResourceManager>().InSingletonScope();
      Bind<IUserInfo>().To<UserInfo>().InTransientScope();
      Bind<IUserSession>().To<UserSession.Impl.UserSession>().InTransientScope();
      Bind<IUserSessionFactory>().ToFactory().InSingletonScope();
      Bind<ILoadingStatus>().To<LoadingStatus>().InSingletonScope();
      Bind<FlyoutsControlRegionAdapter>().ToSelf();
      Bind<IAresCommandRegistry>().To<AresCommandRegistry>().InSingletonScope();
      Bind<IConfigManager>().To<ConfigManager>().InTransientScope();

      Bind<IConfigManagerRegistry>().To<ConfigManagerRegistry>().InSingletonScope();
      // Tell configManagers to load
      Bind<IAresAnalyzerRegistry>().To<AresAnalyzerRegistry>().InSingletonScope();
      Bind<IMachineStateRegistry>().To<MachineStateRegistry>().InSingletonScope();
      Bind<IAresPlannerManagerRegistry>().To<AresPlannerManagerRegistry>().InSingletonScope();
      Bind<IPlannerStatus>().To<PlannerStatus>().InSingletonScope();
      Bind<IApplicationConfiguration>().To<ApplicationConfiguration>().InSingletonScope();
      Bind<IAresConsole>().To<AresConsole>().InSingletonScope();
      Bind<IProjectInfo>().To<ProjectInfo>().InTransientScope();
      Bind<ICurrentConfig>().To<CurrentConfig>().InSingletonScope();
      Bind<IAresCommandPopulator>().To<AresCommandPopulator>().InSingletonScope();
      Bind<IScriptExecutor>().To<ScriptExecutor>().InSingletonScope();
      Bind<ICampaign>().To<Campaign>().InSingletonScope();
      Bind<ICampaignExecutor>().To<CampaignExecutor>().InSingletonScope();
      Bind<IExperimentBatch>().To<ARESExperimentBatch>().InSingletonScope();
      Bind<IDataHub>().To<DataHub.impl.DataHub>().InSingletonScope();
      Bind<IScreenMapper>().To<ScreenMapper>().InSingletonScope();
      Bind<IDbFilter<ExperimentEntity>>().To<ExperimentFilter>().InSingletonScope();
      Bind<IExperimentFilterOptions>().To<ExperimentFilterOptions>().InSingletonScope();
      Bind<IPlannedExperimentBatchInputs>().To<PlannedExperimentBatchInputs>().InTransientScope();
      Bind<IPlannedExperimentInputs>().To<PlannedExperimentInputs>().InTransientScope();
      Bind<IDBChecker>().To<DbChecker>().InSingletonScope();
      Bind<IDbFilterManager>().To<DbFilterManager>().InSingletonScope();
      Bind<IDbConfig>().To<DbConfig>().InSingletonScope();
      Bind<IDbCreator>().To<DbCreator>().InSingletonScope();
      Bind<IDbConfigLoader>().To<DbConfigLoader>().InSingletonScope();
      Bind<IDbColumnCreator>().To<DbColumnCreator>().InSingletonScope();
      Bind<AresContext>().ToMethod(context => AresContext.GetAresContext()).InSingletonScope();
      Bind<IPlanResults>().To<PlanResults>().InSingletonScope();
      Bind<ISelectedPlannersRepository>().To<SelectedPlannersRepository>().InSingletonScope();
      Bind<StepStartParallelCommand>().ToSelf().InTransientScope();
      Bind<StepStartSequentialCommand>().ToSelf().InTransientScope();
      Bind<ErrorHandlingViewModel>().ToSelf().InSingletonScope();
      Bind<IErroredBundle>().To<ErroredBundle>().InTransientScope();
      Bind<BatchResultsViewModel>().ToSelf().InSingletonScope();
      Bind<IAresPlanner>().To<ManualPlanner>().InSingletonScope(); // Should this be here? And I didn't specify scope because the planner module doesn't either
      Bind<BatchInputDataViewModel>().ToSelf().InSingletonScope();
    }
  }
}