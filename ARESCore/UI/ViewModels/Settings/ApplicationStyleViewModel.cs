using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Media;
using ARESCore.DisposePatternHelpers;
using ARESCore.UserSession;
using MahApps.Metro;
using ReactiveUI;

namespace ARESCore.UI.ViewModels.Settings
{
  public class AccentColorMenuData
  {
    public AccentColorMenuData()
    {
      ChangeAccentCommand = ReactiveCommand.Create<Unit, Unit>( u =>
      {
        DoChangeTheme();
        return new Unit();
      } );

    }

    public string Name { get; set; }
    public Brush BorderColorBrush { get; set; }
    public Brush ColorBrush { get; set; }

    public ReactiveCommand<Unit, Unit> ChangeAccentCommand { get; set; }

    protected virtual void DoChangeTheme()
    {
      var theme = ThemeManager.DetectAppStyle( Application.Current );
      var accent = ThemeManager.GetAccent( this.Name );
      ThemeManager.ChangeAppStyle( Application.Current, accent, theme.Item1 );
    }
  }

  public class ApplicationStyleViewModel : BasicReactiveObjectDisposable
  {
    private AccentColorMenuData _accentSelection;

    public ApplicationStyleViewModel()
    {
      // create accent color menu items for the demo
      this.AccentColors = ThemeManager.Accents.Select( a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush } )
        .ToList();
      ChangeToDarkThemeCommand = ReactiveCommand.Create<Unit, Unit>( u =>
      {
        ChangeTheme("BaseDark");
        return new Unit();
      } );
      ChangeToLightThemeCommand = ReactiveCommand.Create<Unit, Unit>( u =>
      {
        ChangeTheme( "BaseLight" );
        return new Unit();
      } );
      var theme = ThemeManager.DetectAppStyle( Application.Current );
      AccentSelection = AccentColors.FirstOrDefault(a => a.Name.Equals( theme.Item2.Name ));
    }

    private void ChangeTheme(string baseName)
    {
      var theme = ThemeManager.DetectAppStyle( Application.Current );
      ThemeManager.ChangeAppStyle( Application.Current, theme.Item2, ThemeManager.GetAppTheme( baseName ) );
      var configFile = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );
      var settings = configFile.AppSettings.Settings;
      if ( settings["Base"] == null )
      {
        settings.Add( "Base", baseName );
      }
      settings["Base"].Value = baseName;
      configFile.Save( ConfigurationSaveMode.Modified );
    }

    public List<AccentColorMenuData> AccentColors { get; set; }

    public ReactiveCommand<Unit, Unit> ChangeToDarkThemeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeToLightThemeCommand { get; set; }



    public AccentColorMenuData AccentSelection
    {
      get { return _accentSelection; }
      set { this.RaiseAndSetIfChanged(ref _accentSelection, value);
        ChangeAccent( value );
      }
    }

    private void ChangeAccent( AccentColorMenuData valueColorBrush )
    {
      var theme = ThemeManager.DetectAppStyle( Application.Current );
      var accent = ThemeManager.GetAccent( valueColorBrush.Name );
      ThemeManager.ChangeAppStyle( Application.Current, accent, theme.Item1 );
      var configFile = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );
      var settings = configFile.AppSettings.Settings;
      if ( settings["Accent"] == null )
      {
        settings.Add( "Accent", valueColorBrush.Name );
      }
      settings["Accent"].Value = valueColorBrush.Name;
      configFile.Save( ConfigurationSaveMode.Modified );
    }
  }
}