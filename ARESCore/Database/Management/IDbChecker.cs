
using System;
using System.Collections.Generic;
using System.Linq;

namespace ARESCore.Database.Management
{
  public enum DBState
  {
    OK,
    BadConnection,
    BadDb
  };
  public interface IDBChecker
  {
    DBState Check( string ip, int port );

    string CheckResults { get; set; }
  }
}
