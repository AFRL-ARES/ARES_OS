using ARESCore.DisposePatternHelpers;
using ARESCore.UI;
using CommonServiceLocator;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AresAdditiveDevicesPlugin.Log.Impl
{
  public class Log : BasicReactiveObjectDisposable, ILog
  {
    private string _fileToWatch;
    private bool _watching = false;
    private FileSystemWatcher _fileWatcher;
    private IDisposable _subscription;
    private readonly IAresConsole _console;
    private long _previousLogPosition = 0;

    public Log(IAresConsole console)
    {
      _console = console;
      var path = @"..\..\..\py\logs\";
      if (Directory.Exists(path))
      {
        FileSystemWatcher fsw = new FileSystemWatcher(path)
        {
          NotifyFilter = NotifyFilters.FileName
        };
        fsw.Created += LogFileCreated;
        fsw.EnableRaisingEvents = true;


        var directory = new DirectoryInfo(path);
        var logFile = directory.GetFiles()
          .OrderByDescending(f => f.LastWriteTime)
          .First();
        _fileToWatch = logFile.Name;
        StartWatching();
      }
    }

    private void LogFileCreated(object sender, FileSystemEventArgs e)
    {
      _fileToWatch = e.Name;
      StartWatching();
    }

    private async void StartWatching()
    {
      if (_watching)
      {
        // disconnect from that file
        _fileWatcher.EnableRaisingEvents = false;
        _fileWatcher.Dispose();
        _watching = false;
      }
      _fileWatcher = new FileSystemWatcher
      {
        Path = @"..\..\..\py\logs\",
        Filter = _fileToWatch,
        NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite //more options
      };
      _fileWatcher.Changed += OnLogChanged;
      _fileWatcher.EnableRaisingEvents = true;
      _subscription?.Dispose();
      while (!File.Exists($@"{_fileWatcher.Path}{_fileToWatch}")) // Breaks the most during VS startup
      {
        await Task.Delay(10);
      }

      await OnLogChangedTask(this, new FileSystemEventArgs(WatcherChangeTypes.Created, _fileWatcher.Path, _fileToWatch));
      ServiceLocator.Current.GetInstance<IAresConsole>().WriteLine();
      _watching = true;
    }

    private async Task OnLogChangedTask(object source, FileSystemEventArgs e)
    {
      var newText = "";
      var path = e.FullPath;

      using (var freader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      using (var reader = new StreamReader(freader))
      {
        freader.Seek(_previousLogPosition, SeekOrigin.Begin);
        if (freader.Length - _previousLogPosition >= _console.MaxLength)
        {
          freader.Seek(freader.Length - _console.MaxLength, SeekOrigin.Current);
        }
        newText = reader.ReadToEnd();
        _previousLogPosition = freader.Position;

      }
      try
      {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
          _console.Document.VerifyAccess();
          //          _console.WriteLine(newText);
          _console.Write(newText);
        });
      }
      catch (Exception)
      {
        // if this happens, it probably means we were shutting down. 
        _subscription.Dispose();
      }
    }

    private async void OnLogChanged(object source, FileSystemEventArgs e)
    {
      await OnLogChangedTask(source, e);
    }
  }
}
