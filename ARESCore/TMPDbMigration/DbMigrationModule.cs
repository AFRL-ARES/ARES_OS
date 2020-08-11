using ARESCore.TMPDbMigration.Migrators;
using ARESCore.TMPDbMigration.Migrators.DataDocs;
using ARESCore.TMPDbMigration.Migrators.DataDocs.Impl;
using ARESCore.TMPDbMigration.Migrators.ExperimentDocs;
using ARESCore.TMPDbMigration.Migrators.ExperimentDocs.Impl;
using ARESCore.TMPDbMigration.Migrators.Impl;
using ARESCore.TMPDbMigration.Migrators.PlannerDocs;
using ARESCore.TMPDbMigration.Migrators.PlannerDocs.Impl;
using ARESCore.TMPDbMigration.UI.ViewModels;
using Ninject.Modules;
using Prism.Mvvm;

namespace ARESCore.TMPDbMigration
{
   public class DbMigrationModule : NinjectModule
   {
      public override void Load()
      {
         Bind<ICollectionMigrator>().To<CollectionMigrator>().InSingletonScope();
         Bind<IExperimentMigrator>().To<ExperimentMigrator>().InSingletonScope();
         Bind<IDataMigrator>().To<DataMigrator>().InSingletonScope();
         Bind<IPlannerMigrator>().To<PlannerMigrator>().InSingletonScope();
         ViewModelLocationProvider.Register<UI.Views.DbMigratorView, DbMigratorViewModel>();
         //provided button on main screen to load data into postgress database
         //ServiceLocator.Current.GetInstance<IRegionManager>().RegisterViewWithRegion("MenuRegion", typeof(UI.Views.DbMigratorView));
      }
   }
}
