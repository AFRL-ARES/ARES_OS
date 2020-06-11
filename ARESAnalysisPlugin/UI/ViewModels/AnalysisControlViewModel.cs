using ARESCore.AnalysisSupport;
using ARESCore.DisposePatternHelpers;
using ARESCore.Registries;
using ARESCore.UI;
using AresFCAnalysisPlugin.Management;
using CommonServiceLocator;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Controls;

namespace AresFCAnalysisPlugin.UI.ViewModels
{
  class AnalysisControlViewModel : BasicReactiveObjectDisposable
  {
    private readonly IAresAnalyzerRegistry _analyzerRegistry;
    private readonly IAresConsole _console;
    private readonly IAnalyzerSetterResolver _analyzerSetterResolver;

    public AnalysisControlViewModel(IAresAnalyzerRegistry analyzerRegistry, IAresConsole console, IPotentialAnalyzerRegistry potentialAnalyzers, IAnalyzerSetterResolver analyzerSetterResolver)
    {
      _analyzerRegistry = analyzerRegistry;
      _console = console;
      _analyzerSetterResolver = analyzerSetterResolver;
      foreach (var analyzer in potentialAnalyzers)
      {
        AvailableAnalyzers.Add(analyzer);
      }

      CreateAnalyzerCommand = ReactiveCommand.Create<IAresAnalyzer>(p => CreateAnalyzer(p));
    }

    private void CreateAnalyzer(IAresAnalyzer analyzer)
    {
      var fname = analyzer.GetType();
      var newAnalyzer = (IAresAnalyzer)ServiceLocator.Current.GetInstance(fname);
      var view = _analyzerSetterResolver.Resolve(newAnalyzer);
      (view.DataContext as AnalyzerSetterViewModel).Analyzer = newAnalyzer;
      _analyzerRegistry.Add(newAnalyzer);
      Setters.Add(view);
    }

    public ObservableCollection<IAresAnalyzer> AvailableAnalyzers { get; set; } = new ObservableCollection<IAresAnalyzer>();

    public ObservableCollection<UserControl> Setters { get; set; } = new ObservableCollection<UserControl>();

    public ReactiveCommand<IAresAnalyzer, Unit> CreateAnalyzerCommand { get; set; }
  }
}