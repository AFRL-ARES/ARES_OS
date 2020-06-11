
namespace ARESCore.UserSession
{
  public interface IUserSession
  {
   string Username { get; set; }
    string SaveDirectory { get; set; }
   string FileNameAndPath { get; set; }
    bool SaveSession();
  }
}
