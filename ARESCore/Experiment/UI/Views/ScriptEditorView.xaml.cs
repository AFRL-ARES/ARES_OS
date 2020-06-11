using ARESCore.Experiment.UI.ViewModels;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace ARESCore.Experiment.UI.Views
{
  /// <summary>
  /// Interaction logic for ScriptEditorView.xaml
  /// </summary>
  public partial class ScriptEditorView : UserControl
  {
    CompletionWindow completionWindow;
    private IList<CompletionData> _aresCompletionList = new List<CompletionData>();
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ScriptEditorView),
      new FrameworkPropertyMetadata { DefaultValue = "", BindsTwoWayByDefault = true, PropertyChangedCallback = TextPropertyCallback });

    private static void TextPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ScriptEditorView sev = (ScriptEditorView)d;
      var newstr = (string)e.NewValue;
      if (newstr != null && !newstr.Equals(sev.textEditor.Document.Text))
      {
        sev.textEditor.Document.Text = newstr;
      }
    }

    public ScriptEditorView()
    {
      DataContext = new ScriptEditorViewModel();
      InitializeComponent();
    }

    public string Text
    {
      get => (string)GetValue(TextProperty);
      set => SetValue(TextProperty, value);
    }

    private IHighlightingDefinition LoadHiglighting()
    {
      // Load our custom highlighting definition
      IHighlightingDefinition customHighlighting;
      using (Stream s = typeof(ScriptEditorView).Assembly.GetManifestResourceStream("ARESCore.Resources.EditorHighlighting.xshd"))
      {
        if (s == null)
          throw new InvalidOperationException("Could not find embedded resource");
        using (XmlReader reader = new XmlTextReader(s))
        {
          customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
          var rules = customHighlighting.MainRuleSet.Rules;
          var dc = (DataContext as ScriptEditorViewModel);
          List<string> deviceCommands = new List<string>();
          List<string> mainCommands = new List<string>();
          foreach (var command in dc.DeviceScriptCommands)
          {
            CompletionData cd = new CompletionData(command.ScriptName, command.HelpString);
            _aresCompletionList.Add(cd);
            if (command.IsPlannable)
            {
              CompletionData cd2 = new CompletionData(command.PlanValueString, "Planned Value for " + command.ScriptName);
              _aresCompletionList.Add(cd2);
            }
            deviceCommands.Add(command.ScriptName);

          }
          foreach (var command in dc.MainScriptCommands)
          {
            CompletionData cd = new CompletionData(command.ScriptName, command.HelpString);
            _aresCompletionList.Add(cd);
            mainCommands.Add(command.ScriptName);
          }
          var scriptRule = new HighlightingRule();
          scriptRule.Color = new HighlightingColor()
          {
            Foreground = new SimpleHighlightingBrush(Colors.CornflowerBlue)
          };

          String[] wordList = mainCommands.ToArray();
          String regex = String.Format(@"\b({0})\w*\b", String.Join("|", wordList));
          scriptRule.Regex = new Regex(regex);
          rules.Add(scriptRule);
          var commandRule = new HighlightingRule();
          commandRule.Color = new HighlightingColor()
          {
            Foreground = new SimpleHighlightingBrush(Colors.LightCoral)
          };

          wordList = deviceCommands.ToArray();
          regex = String.Format(@"\b({0})\w*\b", String.Join("|", wordList));
          commandRule.Regex = new Regex(regex);
          rules.Add(commandRule);
        }
      }
      HighlightingManager.Instance.RegisterHighlighting("ARES", new string[] { ".cool" }, customHighlighting);
      return customHighlighting;
    }

    void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
    {
      if (e.Text.Length > 0 && completionWindow != null)
      {
        if (!char.IsLetterOrDigit(e.Text[0]))
        {
          completionWindow.CompletionList.RequestInsertion(e);
        }
      }
    }

    void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
    {
      if (char.IsLetterOrDigit(e.Text.Last()))
      {
        string currWord = GetWordStart();
        if (currWord.Length == 0)
          return;
        var options = _aresCompletionList.Where(w => w.Text != null && w.Text.StartsWith(currWord));
        if (!options.Any())
          return;
        completionWindow = new CompletionWindow(textEditor.TextArea);
        IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
        foreach (var option in options)
        {
          data.Add(option);
        }
        completionWindow.Show();
        completionWindow.Closed += delegate
        {
          completionWindow = null;
        };
      }

    }

    private string GetWordStart()
    {
      var caret = textEditor.TextArea.Caret.Offset;
      var wordStart = TextUtilities.GetNextCaretPosition(textEditor.Document, caret, LogicalDirection.Backward, CaretPositioningMode.WordStart);
      if (wordStart < 0)
        return "";
      var currWord = textEditor.TextArea.Document.Text.Substring(wordStart, caret - wordStart);
      return currWord;
    }

    private void MainGridLoaded(object sender, RoutedEventArgs e)
    {
      var customHighlighting = LoadHiglighting();
      textEditor.SyntaxHighlighting = customHighlighting;

      textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
      textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
      (DataContext as ScriptEditorViewModel).LiveParse(textEditor.Document.Text);
    }

    private void TextEditor_OnTextChanged(object sender, EventArgs e)
    {
      if (!textEditor.Document.Text.Equals(Text))
      {
        Text = textEditor.Document.Text;
      }
      (DataContext as ScriptEditorViewModel).LiveParse(textEditor.Document.Text);
    }

    private void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      ScrollViewer scv = (ScrollViewer)sender;
      scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
      e.Handled = true;
    }
  }
}
