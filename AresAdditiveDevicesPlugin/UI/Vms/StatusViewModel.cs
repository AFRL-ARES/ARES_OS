using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.DisposePatternHelpers;

namespace AresAdditiveDevicesPlugin.UI.Vms
{
    public class StatusViewModel : BasicReactiveObjectDisposable
    {
        public StatusViewModel()
        {

        }

        public string HelloWorld { get; } = "If I'm displayed, I'm ready to migrate Additive devices as a plugin";
    }
}
