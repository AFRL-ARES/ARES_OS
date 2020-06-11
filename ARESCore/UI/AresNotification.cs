using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Interactivity.InteractionRequest;

namespace ARESCore.UI
{
  public class AresNotification : Notification
  {
    public event EventHandler CloseRequested;

    public void RequestClose()
    {
      CloseRequested?.Invoke( this, EventArgs.Empty );
    }
  }
}
