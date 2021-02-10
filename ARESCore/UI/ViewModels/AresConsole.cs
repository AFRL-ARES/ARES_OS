using ARESCore.DisposePatternHelpers;
using ICSharpCode.AvalonEdit.Document;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ARESCore.UI.ViewModels
{
  public class AresConsole : ReactiveSubscriber, IAresConsole
  {
    private TextDocument _document;

    public AresConsole()
    {
      Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => Document = new TextDocument()));
    }

    private Task DoCleanup()
    {
      var len = Dispatcher.CurrentDispatcher.Invoke(() => Document.Text.Length);
      if (len < MaxLength)
        return Task.CompletedTask;
      // clean it up so we don't start on a random character but a newline
      try
      {
        var newText = Dispatcher.CurrentDispatcher.Invoke(() => Document.Text.Substring(len - MaxLength));
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
        Dispatcher.CurrentDispatcher.Invoke(() =>
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
        Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => Document.Text += text));
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