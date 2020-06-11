using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.Experiment.Scripting;

namespace ARESCore.DeviceSupport
{
  public interface IDeviceScriptData
  {
    string ExpScript { get; set; }
    string InterExpScript { get; set; }
    string CampaignCloseScript { get; set; } 

    string DefaultExpScript();
    string DefaultCampaignCloseScript();
    string DefaultInterExpScript();
  }
}
