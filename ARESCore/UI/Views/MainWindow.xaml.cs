using ARESCore.Database.Views;
using ARESCore.Starter;
using ARESCore.UI.Converters;
using ARESCore.UI.ViewModels;
using ARESCore.UI.Views.Settings;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Ninject;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Animation;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    private readonly IRegionManager _regionManager;
    private Experiment.UI.Views.ExperimentEditorWindow _experimentWindow;
    private bool _loading;

    public MainWindow(IRegionManager regionManager)
    {
      InitializeComponent();

      var screenMapper = AresKernel._kernel.Get<IScreenMapper>();
      Left = screenMapper.PrimaryScreen.WorkingArea.Left;
      Top = screenMapper.PrimaryScreen.WorkingArea.Top;
      _regionManager = regionManager;
      _regionManager.RegisterViewWithRegion("FlyoutRegion", typeof(Flyout));

      if (!_mainGrid.IsEnabled)
      {
        var sb = this.Resources["Blink"] as Storyboard;
        sb.Begin();
      }

      if (!Properties.Settings.Default.LicenseAccepted)
      {
        LicenseWindow lw = new LicenseWindow();
        lw.InitializeComponent();
        lw.Show();
      }
    }

    public bool Loading
    {
      get => (DataContext as MainWindowViewModel).Loading;
      set => (DataContext as MainWindowViewModel).Loading = value;
    }

    private void ShowExperimentPlannerCommand(object sender, RoutedEventArgs e)
    {
      if (_experimentWindow == null || !_experimentWindow.IsLoaded)
      {
        _experimentWindow = new Experiment.UI.Views.ExperimentEditorWindow();
      }
      _experimentWindow.Show();
    }

    private void SettingsButtonClick(object sender, RoutedEventArgs e)
    {
      SettingsWindow asw = new SettingsWindow();
      asw.Owner = this;
      asw.Show();
    }

    private void AboutButtonClick(object sender, RoutedEventArgs e)
    {
      AboutWindow abw = new AboutWindow();
      abw.Owner = this;
      abw.Show();
    }

    private void MainWindowClosed(object sender, EventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
      this.Icon = IconConverter.Convert(PackIconMaterialKind.SwordCross);
    }

    private void ProjectEditorClick(object sender, RoutedEventArgs e)
    {
      ProjectSelectionView window = new ProjectSelectionView();
      window.Owner = this;
      window.Show();

    }

    private void ShowDatabaseViewer(object sender, RoutedEventArgs e)
    {
      DatabaseView window = new DatabaseView();
      window.Owner = this;
      window.Show();
    }

    private void ErrorHandlingView_Loaded(object sender, RoutedEventArgs e)
    {

    }
  }
}
