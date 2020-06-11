using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.DeviceSupport;
using ARESCore.Registries;
using CommonServiceLocator;
using Ninject;
using Prism.Ioc;
using Prism.Regions;

namespace ARESCore.PluginSupport
{
  public abstract class AresModule:IAresModule
  {
    protected readonly IRegionManager _regionManager;
    protected IContainerRegistry _registry;

    protected AresModule( IRegionManager regionManager )
    {
      _regionManager = regionManager;
    }
    public virtual void RegisterTypes( IContainerRegistry containerRegistry )
    {
      _registry = containerRegistry;
    }

    public virtual void OnInitialized( IContainerProvider containerProvider )
    {
      // maybe something we need to do here later...
      InitAndActivateComponents();
    }

    private void InitAndActivateComponents()
    {
      
    }
  }
}
