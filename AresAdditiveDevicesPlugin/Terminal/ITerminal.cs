using ICSharpCode.AvalonEdit.Document;
using System.Threading.Tasks;

namespace AresAdditiveDevicesPlugin.Terminal
{
  public interface ITerminal
  {
    string Text { get; set; }

    void WriteLine(string text);

    void Write(string text);

    void WriteLine();

    void SendMessage(string message);
    Task<bool> Connect(int port);
    void Disconnect(int port);

    TextDocument Document { get; }
  }
}
