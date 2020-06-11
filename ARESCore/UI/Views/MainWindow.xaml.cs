using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using ARESCore.Database.Views;
using ARESCore.Starter;
using ARESCore.UI.Converters;
using ARESCore.UI.Views.Settings;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Ninject;
using Prism.Regions;

namespace ARESCore.UI.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    private readonly IRegionManager _regionManager;
    private Experiment.UI.Views.ExperimentEditorWindow _experimentWindow;

    public MainWindow( IRegionManager regionManager )
    {
      InitializeComponent();
      
      var screenMapper = AresKernel._kernel.Get<IScreenMapper>();
      Left = screenMapper.PrimaryScreen.WorkingArea.Left;
      Top = screenMapper.PrimaryScreen.WorkingArea.Top;
      _regionManager = regionManager;
      _regionManager.RegisterViewWithRegion( "FlyoutRegion", typeof( Flyout ) );
      
      if(!_mainGrid.IsEnabled)
      {
        var sb = this.Resources["Blink"] as Storyboard;
        sb.Begin();
      }
    }

    private void ShowExperimentPlannerCommand( object sender, RoutedEventArgs e )
    {
      if ( _experimentWindow == null || !_experimentWindow.IsLoaded)
      {
        _experimentWindow = new Experiment.UI.Views.ExperimentEditorWindow();
      }
      _experimentWindow.Show();
    }

    private void SettingsButtonClick( object sender, RoutedEventArgs e )
    {
      SettingsWindow asw = new SettingsWindow();
      asw.Owner = this;
      asw.Show();
    }

    private void MainWindowClosed( object sender, EventArgs e )
    {
      Application.Current.Shutdown();
    }

    private void MainWindowLoaded( object sender, RoutedEventArgs e )
    {
      this.Icon = IconConverter.Convert( PackIconMaterialKind.SwordCross );
    }

    private void ProjectEditorClick( object sender, RoutedEventArgs e )
    {
        ProjectSelectionView window = new ProjectSelectionView();
        window.Owner = this;
        window.Show();

    }

    private void ShowDatabaseViewer( object sender, RoutedEventArgs e )
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
