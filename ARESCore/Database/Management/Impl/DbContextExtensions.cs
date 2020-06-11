using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARESCore.Database.Management.Impl
{
  public static class DbContextExtensions
  {
    public static IEnumerable<T> SetOf<T>( this DbContext dbContext ) where T : class
    {
      List<Type> types = new List<Type>();
      foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
      {
        types.AddRange( assembly.GetTypes() );
      }
      return types.Where( type => typeof( T ).IsAssignableFrom( type ) && !type.IsInterface )
        .SelectMany( t => Enumerable.Cast<T>( dbContext.Set( t ) ));
    }
  }
}
