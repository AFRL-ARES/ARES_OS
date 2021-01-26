using ARESCore.DisposePatternHelpers;
using ICSharpCode.AvalonEdit.Document;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ARESCore.UI.ViewModels
{
  public class AresConsole : ReactiveSubscriber, IAresConsole
  {
    private TextDocument _document;

    public AresConsole()
    {
      Application.Current.Dispatcher.BeginInvoke(new Action(() => Document = new TextDocument()));
    }

    private Task DoCleanup()
    {
      var len = Application.Current.Dispatcher.Invoke(() => Document.Text.Length);
      if (len < MaxLength)
        return Task.CompletedTask;
      // clean it up so we don't start on a random character but a newline
      try
      {
        var newText = Application.Current.Dispatcher.Invoke(() => Document.Text.Substring(len - MaxLength));
        int truncateIdx = -1;
        for (int i = 0; i < newText.Length; i++)
        {
          if (newText[i].Equals('\n'))
          {
            truncateIdx = i;
            break;
          }
        }
        newText = newText.Substring(truncateIdx);
        Application.Current.Dispatcher.Invoke(() =>
        {
          Document.Text = newText;
          Document.UndoStack.ClearAll();
        });

      }
      catch (Exception)
      {
        //text was truncated by different thread
      }
      return Task.CompletedTask;
    }

    public TextDocument Document
    {
      get => _document;
      set => this.RaiseAndSetIfChanged(ref _document, value);
    }

    public void WriteLine(string text)
    {
      Write(text + "\n");
    }

    public void Write(string text)
    {
      try
      {
        Application.Current.Dispatcher.BeginInvoke((Action)(() => Document.Text += text));
        DoCleanup();
      }
      catch (Exception)
      {
        // This empty catch block is on purpose: if this happened, it means we were shutting down while trying to write.
      }
    }

    public void WriteLine()
    {
      WriteLine("");
    }

    public int MaxLength { get; } = 8191;
  }
}