using ARESCore.DisposePatternHelpers;
using ICSharpCode.AvalonEdit.Document;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ARESCore.UI.ViewModels
{
    public class AresConsole : BasicReactiveObjectDisposable, IAresConsole
    {
        private readonly string _text;
        private TextDocument _document;
        private readonly int _maxLength = 8191; // this matches the windows console

        public AresConsole()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Document = new TextDocument()));
            //      Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(x => CleanUpConsole());
        }

        //    private void CleanUpConsole()
        //    {
        //      Application.Current.Dispatcher.BeginInvoke(new Action(() => DoCleanup()));
        //    }

        private Task DoCleanup()
        {
            var len = Application.Current.Dispatcher.Invoke(() => Document.Text.Length);
            if (len < _maxLength)
                return Task.CompletedTask;
            // clean it up so we don't start on a random character but a newline
            try
            {
                var newText = Application.Current.Dispatcher.Invoke(() => Document.Text.Substring(len - _maxLength));
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
            catch (Exception e)
            {
                //text was truncated by different thread
            }
            return Task.CompletedTask;
        }

        //    public string Text
        //    {
        //      get => _text;
        //      set => this.RaiseAndSetIfChanged(ref _text, value);
        //    }

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

        public int MaxLength => _maxLength;
    }
}