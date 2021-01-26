using ARESCore.DisposePatternHelpers;
using ControlzEx.Theming;
using ReactiveUI;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Media;

namespace ARESCore.UI.ViewModels.Settings
{
  public class AccentColorMenuData
  {
    public AccentColorMenuData()
    {
      ChangeAccentCommand = ReactiveCommand.Create<Unit, Unit>(u =>
     {
       DoChangeTheme();
       return new Unit();
     });

    }

    public string Name { get; set; }
    public Brush ColorBrush { get; set; }

    public ReactiveCommand<Unit, Unit> ChangeAccentCommand { get; set; }

    protected virtual void DoChangeTheme()
    {
      var theme = ThemeManager.Current.DetectTheme(Application.Current);
      ThemeManager.Current.ChangeTheme(Application.Current, theme);
    }
  }

  public class ApplicationStyleViewModel : ReactiveSubscriber
  {
    private AccentColorMenuData _accentSelection;

    public ApplicationStyleViewModel()
    {
      // create accent color menu items for the demo
      var accentNames = new List<string>
      {
        "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Teal", "Cyan", "Indigo", "Violet",
        "Pink", "Magenta", "Crimson", "Yellow", "Brown", "Olive", "Sienna"
      };
      AccentColors = new List<AccentColorMenuData>();
      foreach (var accentName in accentNames)
      {
        var conversion = ColorConverter.ConvertFromString(accentName);
        if (conversion == null)
          continue;
        var cb = new SolidColorBrush((Color)conversion);
        var menuItem = new AccentColorMenuData { Name = accentName, ColorBrush = cb };
        AccentColors.Add(menuItem);
      }

      ChangeToDarkThemeCommand = ReactiveCommand.Create<Unit, Unit>(u =>
     {
       ChangeTheme("Dark");
       return new Unit();
     });
      ChangeToLightThemeCommand = ReactiveCommand.Create<Unit, Unit>(u =>
     {
       ChangeTheme("Light");
       return new Unit();
     });
      var theme = ThemeManager.Current.DetectTheme(Application.Current);
      AccentSelection = AccentColors.FirstOrDefault(a => theme.Name.EndsWith(a.Name));
    }

    private void ChangeTheme(string baseName)
    {
      var theme = ThemeManager.Current.DetectTheme(Application.Current);
      ThemeManager.Current.ChangeThemeBaseColor(Application.Current, baseName);
      var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var settings = configFile.AppSettings.Settings;
      if (settings["Base"] == null)
      {
        settings.Add("Base", baseName);
      }
      settings["Base"].Value = baseName;
      configFile.Save(ConfigurationSaveMode.Modified);
    }

    public List<AccentColorMenuData> AccentColors { get; set; }

    public ReactiveCommand<Unit, Unit> ChangeToDarkThemeCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ChangeToLightThemeCommand { get; set; }



    public AccentColorMenuData AccentSelection
    {
      get { return _accentSelection; }
      set
      {
        this.RaiseAndSetIfChanged(ref _accentSelection, value);
        ChangeAccent(value);
      }
    }

    private void ChangeAccent(AccentColorMenuData valueColorBrush)
    {
      var theme = ThemeManager.Current.DetectTheme(Application.Current);
      ThemeManager.Current.ChangeThemeColorScheme(Application.Current, valueColorBrush.Name);
      var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var settings = configFile.AppSettings.Settings;
      if (settings["Accent"] == null)
      {
        settings.Add("Accent", valueColorBrush.Name);
      }
      settings["Accent"].Value = valueColorBrush.Name;
      configFile.Save(ConfigurationSaveMode.Modified);
    }
  }
}