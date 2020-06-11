using System.Linq;
using ARESCore.AnalysisSupport;
using ARESCore.Commands;
using ARESCore.Configurations;
using ARESCore.DeviceSupport;
using ARESCore.PlanningSupport.Impl;
using ARESCore.Registries;
using ARESCore.UI;
using ARESCore.UserSession;
using CommonServiceLocator;
using MahApps.Metro.IconPacks;
using Ninject;

namespace ARESCore.Starter
{
   internal class AresStarter : IAresStarter
   {
      private ILoadingStatus _loadingStatus;
      private IKernel _kernel;
      private IAresConsole _console;

      public void Start()
      {
         _kernel = AresKernel._kernel;
         _console = _kernel.Get<IAresConsole>();
         PerformEagerInstantiations();
         _loadingStatus = _kernel.Get<ILoadingStatus>();
         _loadingStatus.Icon = PackIconMaterialKind.Database;
         // this (below) was in the old splash screen. I don't think we need it...
         var all = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceNames();
         foreach (var s in all)
         {
            _console.WriteLine(s);
         }
         _loadingStatus.StatusInfo = "Loading Database ... ";

         // Start the ConnectionManager process 
         _loadingStatus.StatusInfo = "Starting User Setup ... ";
         var currentConfig = _kernel.Get<ICurrentConfig>();
         var loadedSession = AresKernel._kernel.Get<IUserSessionFactory>().CreateSession(currentConfig.User.SaveFileName);
         loadedSession.FileNameAndPath = currentConfig.User.SaveFileName;
         loadedSession.SaveDirectory = currentConfig.User.SaveDirectory;
         loadedSession.Username = currentConfig.User.Username;
         var userList = _kernel.Get<IApplicationConfiguration>().UserList;
         if (!userList.Any(u => u.Username.Equals(currentConfig.User.Username)))
            userList.Add(currentConfig.User);
         currentConfig.UserSession = loadedSession;
         var projList = _kernel.Get<IApplicationConfiguration>().ProjectList;
         if (projList != null && projList.Count > 0)
         {
            currentConfig.Project = projList.OrderBy(p => p.LastLoadedDate).Last();
         }
         // detect and load project
         SaveUserSession("New UserSession created.", null, false);
         SaveAppConfig("New UserSession created.", null, false);

         _loadingStatus.StatusInfo = "Loading Device Configurations...";
         var reg = AresKernel._kernel.Get<IConfigManagerRegistry>();
         foreach (var cfgMgr in reg)
         {
            cfgMgr.LoadConfigs();
         }
         var devices = ServiceLocator.Current.GetAllInstances<IAresDevice>();

         foreach (IAresDevice device in devices)
         {
            device.Activate();
         }
      }

      private void PerformEagerInstantiations()
      {
         _kernel.Get<IAresCommandPopulator>().Populate();
         var analysisRegistry = _kernel.Get<IAresAnalyzerRegistry>();
         analysisRegistry.Add(_kernel.Get<CustomExperimentAnalysis>());
         var plannerRegistry = _kernel.Get<IAresPlannerManagerRegistry>();
         plannerRegistry.Add(_kernel.Get<ManualPlannerManager>());
      }

      public bool SaveUserSession(string failedActionReason, string successActionReason = null, bool logFail = true)
      {
         if (!_kernel.Get<ICurrentConfig>().UserSession.SaveSession())
         {
            // couldn't save....
            return false;
         }
         if (successActionReason != null)
         {
            // did save ...
         }
         return true;
      }

      public bool SaveAppConfig(string failedActionReason, string successActionReason = null, bool logFail = true)
      {
         var appconfig = AresKernel._kernel.Get<IApplicationConfiguration>();
         if (!appconfig.SaveConfig(""))
         {
            // couldn't save...
            return false;
         }
         if (successActionReason != null)
         {
            // did save ...
         }
         return true;
      }
   }
}