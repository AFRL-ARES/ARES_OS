using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.PlanningSupport.Impl
{
  internal class PlannerStatus : BasicReactiveObjectDisposable, IPlannerStatus
  {
    private string _statusText;
    private ContentControl _image;

    public string StatusText
    {
      get => _statusText;
      set => this.RaiseAndSetIfChanged( ref _statusText, value );
    }

    public ContentControl Image
    {
      get => _image;
      set => this.RaiseAndSetIfChanged( ref _image, value );
    }
  }
}