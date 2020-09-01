using ARESCore.Database.Management.Impl;
using ARESCore.Database.Tables;

namespace AresCNTDevicesPlugin.Laser.Database
{
  public class LaserDbRepository: GenericDbRepository<LaserDbEntity>
  {
    public LaserDbRepository( AresContext context ) : base( context )
    {
    }
  }
}
