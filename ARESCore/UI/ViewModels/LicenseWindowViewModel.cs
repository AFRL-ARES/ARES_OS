using ARESCore.DisposePatternHelpers;
using ReactiveUI;
using System.IO;
using System.Reactive;

namespace ARESCore.UI.ViewModels
{
  public class LicenseWindowViewModel : ReactiveSubscriber
  {
    private string _license;
    public string LicenseText
    {
      get => _license;
      set => this.RaiseAndSetIfChanged(ref _license, value);
    }
    public ReactiveCommand<Unit, Unit> AcceptClick { get; set; }
    public ReactiveCommand<Unit, Unit> DeclineClick { get; set; }
    public LicenseWindowViewModel()
    {
      LicenseText = File.ReadAllText("..\\..\\..\\..\\..\\documents\\License.md");
      AcceptClick = ReactiveCommand.Create<Unit, Unit>(t =>
      {
        Properties.Settings.Default.LicenseAccepted = true;
        Properties.Settings.Default.Save();
        return new Unit();
      });
      DeclineClick = ReactiveCommand.Create<Unit, Unit>(t =>
      {
        App.Current.Shutdown();
        return new Unit();
      });
    }
  }
}