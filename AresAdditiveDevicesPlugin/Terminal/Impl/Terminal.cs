using ARESCore.DisposePatternHelpers;
using ICSharpCode.AvalonEdit.Document;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AresAdditiveDevicesPlugin.Terminal.Impl
{
  public class Terminal : BasicReactiveObjectDisposable, ITerminal
  {
    private string _text;
    private TextDocument _document;
    private readonly int _maxLength = 8191; // this matches the windows console
                                            //    private readonly int _maxLength = 3000;
    private readonly Dictionary<int, ClientPair> _clients = new Dictionary<int, ClientPair>();
    private readonly Stack<int> _responsePorts = new Stack<int>();

    public Terminal()
    {
      Document = new TextDocument();
      Document.UndoStack.SizeLimit = 5;

      Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(x => CleanUpConsole());
    }

    private void CleanUpConsole()
    {
      Application.Current.Dispatcher.Invoke(DoCleanup);
    }

    private void DoCleanup()
    {
      var len = Document.Text.Length;
      if (len < _maxLength)
        return;
      // clean it up so we don't start on a random character but a newline
      var newText = Document.Text.Substring(len - _maxLength, _maxLength);
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
      Document.Text = newText;
      Document.UndoStack.ClearAll();
    }

    public async Task<bool> Connect(int port)
    {
      if (!_clients.ContainsKey(port))
      {
        var client = new TcpClient();
        _clients.Add(port, new ClientPair(client, null));
      }
      else
      {
        //        if ( _clients[port].Client.Connected )
        if (_clients[port].ShouldBeActive)
        {
          return false; // Already being used for something
        }
      }

      var attempts = 0;
      bool connected = false;

      await Task.Run(() =>
      {
        while (!connected && attempts < 6)
        {
          _clients[port].ShouldBeActive = true;
          try
          {
            attempts++;
            _clients[port].Client = new TcpClient("localhost", port);
            Application.Current.Dispatcher.Invoke(() => connected = _clients[port].Client.Connected);

          }
          catch (Exception)
          {
            if (attempts >= 6)
            {
              _clients[port].ShouldBeActive = false;
              WriteLine($"Failed to connect to port {port}");
              return;
            }
          }
        }
        if (connected)
        {
          _clients[port].Thread = new Thread(() => Run(port)) { IsBackground = true };
          _clients[port].Thread.Start();
        }
      });
      return connected;

    }

    private void Run(int port)
    {
      var buffer = new byte[1024];
      TcpClient client;
      while ((client = _clients[port].Client).Connected && _clients[port].ShouldBeActive)
      {
        try
        {
          using (var stream = client.GetStream())
          {
            int length;
            while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
              var data = new byte[length];
              Array.Copy(buffer, 0, data, 0, length);
              var message = Encoding.ASCII.GetString(data);
              WriteLine(message);
              if (message.Contains("[Y/") || message.Contains("[Y]") || message.Contains("/N]") || message.Contains("[N]"))
              {
                _responsePorts.Push(_clients.FirstOrDefault(pair => pair.Value.Client == client).Key);
              }
            }
          }
        }
        catch (Exception)
        {

        }
      }
      Disconnect(port);
    }

    public void SendMessage(string message)
    {
      if (!_responsePorts.Any())
      {
        return;
      }
      var client = _clients[_responsePorts.Pop()].Client;
      if (client == null)
      {
        return;
      }
      try
      {
        var stream = client.GetStream();
        if (!stream.CanWrite)
          return;
        var data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
      }
      catch (Exception)
      {

      }
    }

    public void Disconnect(int port)
    {
      _clients[port].ShouldBeActive = false;
      _clients[port].Client?.Close();
      _clients[port].Thread = null;
    }

    public string Text
    {
      get => _text;
      set => this.RaiseAndSetIfChanged(ref _text, value);
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
        Application.Current.Dispatcher.Invoke(() =>
        {
          Document.Text += text;
          DoCleanup();
        });
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

    private class ClientPair
    {
      public TcpClient Client { get; set; }
      public Thread Thread { get; set; }
      public bool ShouldBeActive { get; set; }

      public ClientPair() { }
      public ClientPair(TcpClient client, Thread thread)
      {
        Client = client;
        Thread = thread;
        ShouldBeActive = false;
      }
    }

  }
}
