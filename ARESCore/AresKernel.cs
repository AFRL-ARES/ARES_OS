using ARESCore.Experiment.UI.ViewModels;
using ARESCore.PluginSupport;
using ARESCore.TMPDbMigration;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.Syntax;
using System.IO;
using System.Reflection;
using ARESCore.Experiment.UI.Views;
using Ninject.Syntax;
using Prism.Ninject;

namespace ARESCore
{
  public class AresKernel
  {
    public static IKernel _kernel;

    private static void BindAll(IFromSyntax scanner)
    {
      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var assemblies = scanner.FromAssembliesInPath(path);
      var allclasses = assemblies.SelectAllClasses();
      var plugins = allclasses.InheritedFrom<IAresPlugin>();
      plugins.BindAllInterfaces().Configure(c => c.InSingletonScope());
    }

    public void Apply(IKernel kernel)
    {
      _kernel = kernel;
      _kernel.Load(new ARESCoreModule(), new DbMigrationModule() // TODO FIXME delete dbmigration 
        );
      _kernel.Bind(BindAll);
      _kernel.RegisterTypeForNavigation<CampaignExecutionView, CampaignExecutionViewModel>();
      _kernel.RegisterTypeForNavigation<BatchResultsView, BatchResultsViewModel>();
    }

    public CampaignExecutionViewModel CampaignExecutionViewModel => _kernel.Get<CampaignExecutionViewModel>();
    public BatchResultsViewModel BatchResultsViewModel => _kernel.Get<BatchResultsViewModel>();
  }
}
