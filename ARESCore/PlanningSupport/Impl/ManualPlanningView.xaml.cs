using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace ARESCore.PlanningSupport.Impl
{
  /// <summary>
  /// Interaction logic for ManualPlanningView.xaml
  /// </summary>
  public partial class ManualPlanningView : UserControl
  {
    private readonly ManualPlanningViewModel _viewModel = new ManualPlanningViewModel( DialogCoordinator.Instance );
    public ManualPlanningView()
    {
      InitializeComponent();
      DataContext = _viewModel;
    }

    private void OpenFileButtonClick( object sender, RoutedEventArgs e )
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
      if ( openFileDialog.ShowDialog() == true )
        _viewModel.FilePathText = openFileDialog.FileName;
    }
  }
}