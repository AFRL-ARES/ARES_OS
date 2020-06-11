using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ARESCore.Experiment.UI.ViewModels;
using Microsoft.Win32;

namespace ARESCore.Experiment.UI.Views
{
  /// <summary>
  /// Interaction logic for BatchScriptReviewView.xaml
  /// </summary>
  public partial class BatchScriptReviewView : UserControl
  {
    public BatchScriptReviewView()
    {
      InitializeComponent();
    }

    private string TryLoadText()
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.DefaultExt = ".txt";
      dlg.Filter = "Text documents (.txt)|*.txt";
      var result = dlg.ShowDialog();
      if ( result == true )
      {
        string filename = dlg.FileName;
        var text = File.ReadAllText( filename );
        return text;
      }
      else
      {
        return null;
      }
    }
    private void TrySaveText( string scriptToSave )
    {
      var dlg = new SaveFileDialog();
      dlg.DefaultExt = ".txt";
      dlg.Filter = "Text documents (.txt)|*.txt";
      var result = dlg.ShowDialog();
      if ( result == true )
      {
        string filename = dlg.FileName;
        File.WriteAllText( filename, scriptToSave );
      }
    }

    private void LoadExperimentScriptClicked( object sender, RoutedEventArgs e )
    {
      string text = TryLoadText();
      if (text != null)
      ( DataContext as BatchScriptReviewViewModel ).CurrentCampaign.ExpScript = text;
    }

    private void SaveExperimentScriptClicked( object sender, RoutedEventArgs e )
    {
      TrySaveText( ( DataContext as BatchScriptReviewViewModel ).CurrentCampaign.ExpScript );
    }

    private void LoadInterScriptClicked( object sender, RoutedEventArgs e )
    {
      string text = TryLoadText();
      if ( text != null )
        ( DataContext as BatchScriptReviewViewModel ).CurrentCampaign.InterExpScript = text;
    }

    private void SaveInterScriptClicked( object sender, RoutedEventArgs e )
    {
      TrySaveText( ( DataContext as BatchScriptReviewViewModel ).CurrentCampaign.InterExpScript );
    }

    private void LoadCloseoutScriptClicked( object sender, RoutedEventArgs e )
    {
      string text = TryLoadText();
      if ( text != null )
        ( DataContext as BatchScriptReviewViewModel ).CurrentCampaign.InterExpScript = text;
    }

    private void SaveCloseoutScriptClicked( object sender, RoutedEventArgs e )
    {
      TrySaveText( ( DataContext as BatchScriptReviewViewModel ).CurrentCampaign.CampaignCloseScript );
    }
  }
}
