using ICSharpCode.AvalonEdit.Document;

namespace ARESCore.UI
{
  public interface IAresConsole
  {
    void WriteLine(string text);

    void Write(string text);

    void WriteLine();

    int MaxLength { get; }

    TextDocument Document { get; }
  }
}
