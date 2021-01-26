using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARESCore.Configurations;
using ARESCore.DisposePatternHelpers;
using ReactiveUI;

namespace ARESCore.UserSession.Impl
{
  internal class CurrentConfig:ReactiveSubscriber, ICurrentConfig
  {
    private IUserInfo _user;
    private IProjectInfo _projectInfo;
    private IUserSession _userSession;
    private double _timerPrecision = 1.0;

    public IUserInfo User
    {
      get => _user;
      set => this.RaiseAndSetIfChanged( ref _user, value );
    }

    public IProjectInfo Project
    {
      get => _projectInfo;
      set => this.RaiseAndSetIfChanged( ref _projectInfo, value );
    }

    public IUserSession UserSession
    {
      get => _userSession;
      set => this.RaiseAndSetIfChanged( ref _userSession, value);
    }

    public double TimerPrecision
    {
      get => _timerPrecision;
      set => this.RaiseAndSetIfChanged(ref _timerPrecision, value);
    }
  }
}
