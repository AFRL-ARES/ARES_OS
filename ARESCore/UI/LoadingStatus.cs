using ARESCore.DisposePatternHelpers;
using MahApps.Metro.IconPacks;
using ReactiveUI;
using System.Windows;

namespace ARESCore.UI
{
  internal class LoadingStatus : BasicReactiveObjectDisposable, ILoadingStatus
  {
    private string _statusInfo;
    private PackIconMaterialKind _iconKind = PackIconMaterialKind.Cookie;

    public string StatusInfo
    {
      get => _statusInfo;
      set => Application.Current.Dispatcher.Invoke(delegate () { this.RaiseAndSetIfChanged(ref _statusInfo, value); });
    }

    public PackIconMaterialKind Icon
    {
      get => _iconKind;
      set => Application.Current.Dispatcher.Invoke(delegate () { this.RaiseAndSetIfChanged(ref _iconKind, value); });
    }
  }
}
