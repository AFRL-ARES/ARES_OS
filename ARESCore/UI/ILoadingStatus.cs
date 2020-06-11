using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.IconPacks;

namespace ARESCore.UI
{
  public interface ILoadingStatus
  {
    string StatusInfo { get; set; }

    PackIconMaterialKind Icon { get; set; }
  }
}
