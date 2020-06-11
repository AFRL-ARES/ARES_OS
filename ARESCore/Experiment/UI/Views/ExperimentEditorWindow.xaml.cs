using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ARESCore.Experiment.Results;
using ARESCore.Extensions;
using ARESCore.UI.Converters;
using DynamicData.Binding;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Ninject;

namespace ARESCore.Experiment.UI.Views
{
  /// <summary>
  /// Interaction logic for ExperimentEditorWindow.xaml
  /// </summary>
  public partial class ExperimentEditorWindow : MetroWindow
  {
    private ICampaignExecutionSummary _campaignExecutionSummary;
    public ExperimentEditorWindow()
    {
      InitializeComponent();
    }

    private void ExperimentEditorWindowLoaded( object sender, RoutedEventArgs e )
    {
      var icon = IconConverter.Convert( PackIconMaterialKind.Beaker );
      this.Icon = icon;
      _campaignExecutionSummary = AresKernel._kernel.Get<ICampaignExecutionSummary>();
      _campaignExecutionSummary.Subscribe( t => t.Status, CheckStatus );
    }

    private void CheckStatus()
    {
      if ( _campaignExecutionSummary.Status == ExecutionStatus.EXECUTING )
        IsCloseButtonEnabled = false;
      else
      {
        IsCloseButtonEnabled = true;
      }
    }
  }
}
