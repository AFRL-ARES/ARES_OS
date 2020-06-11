using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARESCore.Database.Management.Impl
{
  [AttributeUsage( AttributeTargets.Class )]
  public class PersistentAttribute : Attribute
  {
  }
}
