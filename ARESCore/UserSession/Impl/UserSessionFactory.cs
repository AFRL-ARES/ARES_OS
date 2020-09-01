using ARESCore.UI;
using Newtonsoft.Json;
using Ninject;
using System;
using System.IO;

namespace ARESCore.UserSession.Impl
{
  public class UserSessionFactory : IUserSessionFactory
  {
    public IUserSession CreateSession(string sessionPath)
    {
      if (File.Exists(sessionPath))
      {
        try
        {
          string sessionJson = File.ReadAllText(sessionPath);
          return JsonConvert.DeserializeObject<IUserSession>(sessionJson);
        }
        catch (Exception ex)
        {
          AresKernel._kernel.Get<IAresConsole>().WriteLine(ex.Message);
          return null;
        }
      }
      return null;
    }
  }
}