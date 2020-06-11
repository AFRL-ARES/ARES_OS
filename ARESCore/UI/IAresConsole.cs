using ICSharpCode.AvalonEdit.Document;

namespace ARESCore.UI
{
  public interface IAresConsole
  {
    //    string Text { get; set; }

    void WriteLine(string text);

    void Write(string text);

    void WriteLine();

    int MaxLength { get; }

    TextDocument Document { get; }
  }
}
