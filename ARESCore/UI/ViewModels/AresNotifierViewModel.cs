using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace ARESCore.UI.ViewModels
{
 public class AresNotifierViewModel: BindableBase, IInteractionRequestAware
  {
    private INotification _notification;

    public INotification Notification
    {
      get { return _notification; }
      set
      {
        if ( value is AresNotification )
        {
          _notification = (AresNotification)value;
          RaisePropertyChanged( nameof(Notification) );
        }
      }
    }

    public Action FinishInteraction { get; set; }
  }
}
