using ARESCore.Database.Management;
using ARESCore.Starter;
using ARESCore.UI;
using ARESCore.UI.Views;
using ARESCore.UserSession;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Ninject;
using Prism.Regions;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ARESCore
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : PrismApplication
  {
    private UI.Views.MainWindow _shell;
    private AresSplashScreen _splash;
    private ConsoleWindow _console;
    private IPAddressPortView _ipPortWindow;

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    protected override Window CreateShell()
    {
      AresKernel kernel = FindResource("_aresKernel") as AresKernel;
      kernel.Apply(Container.GetContainer());
      var checker = Container.Resolve<IDBChecker>();
      _splash = Container.Resolve<AresSplashScreen>();
      var config = Container.Resolve<IDbConfigLoader>().Load();
      DBState st;
      while ((st = checker.Check(config.Ip, config.Port)) == DBState.BadConnection)
      {
        _ipPortWindow = Container.Resolve<IPAddressPortView>();
        var dres = _ipPortWindow.ShowDialog();
        if (!dres.HasValue || !dres.Value)
        {
          return Container.Resolve<AppKiller>();
        }
      }
      if (st == DBState.BadDb)
      {
        var creator = Container.Resolve<IDbCreator>();
        creator.Create();
      }

      if (!ARESCore.Properties.Settings.Default.LicenseAccepted)
      {
        LicenseWindow lw = new LicenseWindow();
        lw.InitializeComponent();
        lw.ShowDialog();
      }

      var userSelection = Container.Resolve<UserSessionSelection>();
      var dr = userSelection.ShowDialog();
      if (dr.HasValue && dr.Value)
      {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();
        _shell = Container.Resolve<UI.Views.MainWindow>();
        _console = Container.Resolve<ConsoleWindow>();
        _console.Show();
        _shell.Loading = true;
        _shell.Show();
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();

        Thread t = new Thread(() => DoLoad());
        t.SetApartmentState(ApartmentState.STA);
        t.IsBackground = true;
        t.Start();

        return _shell;
      }
      var killer = Container.Resolve<AppKiller>();
      return killer;
    }


    private Task DoLoad()
    {
      var starter = Container.Resolve<IAresStarter>();

      AddRegionMappings();
      starter.Start();
      InitializeModules();
      Application.Current.Dispatcher.BeginInvoke(() => _shell.Loading = false);


      return Task.FromResult(true);
    }

    protected void AddRegionMappings()
    {
      Container.Resolve<RegionAdapterMappings>().RegisterMapping(typeof(FlyoutsControl), Container.Resolve<FlyoutsControlRegionAdapter>());
    }

    protected override IModuleCatalog CreateModuleCatalog()
    {
      var modulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var catalog = new DirectoryModuleCatalog() { ModulePath = modulePath };
      catalog.Load();
      return catalog;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      // create custom accents
      var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var settings = configFile.AppSettings.Settings;
      string accentName = "Red";
      string baseName = "Dark";
      if (settings["Accent"] != null)
        accentName = settings["Accent"].Value;
      if (settings["Base"] != null)
        baseName = settings["Base"].Value;
      if (baseName.ToLower().StartsWith("base"))
      {
        ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Green");
      }
      else
      {

        ThemeManager.Current.ChangeTheme(Application.Current, baseName + "." + accentName);
      }

      base.OnStartup(e);
    }
  }
}