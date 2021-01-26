using System.IO;
using System.Reactive;
using System.Text.RegularExpressions;
using ARESCore.Database.Management;
using ARESCore.DisposePatternHelpers;
using ARESCore.Util;
using Ninject;
using Prism.Ioc;
using ReactiveUI;

namespace ARESCore.UI.ViewModels
{
  public class IPAddressPortViewModel : ReactiveSubscriber
  {
    public IContainerProvider _container;
    private readonly IDBChecker _checker;
    private IDbConfig _config;

    public IPAddressPortViewModel( IDBChecker checker, IDbConfig config )
    {
      _checker = checker;
      Config = config;
      SaveCommand = ReactiveCommand.Create<Unit>( _ => Save() );
    }

    private void Save()
    {
      AresKernel._kernel.Get<IDbConfigLoader>().Save();
    }

    private bool IsValidIp( string Address )
    {
      //Match pattern for IP address    
      string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])((\.|\,|\-)([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
      Regex check = new Regex( pattern );

      //check to make sure an ip address was provided    
      if ( string.IsNullOrEmpty( Address ) )
        return false;

      return check.IsMatch( Address, 0 );
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }

    public IDbConfig Config
    {
      get => _config;
      set => this.RaiseAndSetIfChanged(ref _config , value);
    }
  }
}