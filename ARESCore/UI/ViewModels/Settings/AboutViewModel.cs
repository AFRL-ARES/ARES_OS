using System;
using System.ComponentModel;
using System.IO;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ARESCore.DisposePatternHelpers;
using ARESCore.Registries;
using ARESCore.UI.Helpers;
using ARESCore.UI.Views.Settings;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Services.Dialogs;
using ReactiveUI;

namespace ARESCore.UI.ViewModels.Settings
{
  public class AboutViewModel : BasicReactiveObjectDisposable
  {
    private string _readme;
    public string ReadmeText
    {
      get => _readme;
      set => this.RaiseAndSetIfChanged(ref _readme, value);
    }
    public AboutViewModel()
    {
      try
      {
        ReadmeText = File.ReadAllText("..\\..\\..\\..\\documents\\README.md") +
                     File.ReadAllText("..\\..\\..\\..\\documents\\License.md");
      }
      catch (Exception)
      {
        ReadmeText = "Had an error loading the readme documents.";
      }
    }
  }
}