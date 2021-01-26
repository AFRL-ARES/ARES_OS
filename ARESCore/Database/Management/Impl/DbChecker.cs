using System;
using System.Collections.Generic;
using System.Linq;
using ARESCore.DisposePatternHelpers;
using Npgsql;
using ReactiveUI;

namespace ARESCore.Database.Management.Impl
{
 
  internal class DbChecker: ReactiveSubscriber, IDBChecker
  {
    private string _checkResults;

    public DBState Check( string ip, int port )
    {
      var connstr = "Server=" + ip + ";Port=" + port + ";User Id=postgres;Password=a";
      bool connOk = false;
      using ( var conn = new NpgsqlConnection( connstr ) )
      {
        try
        {
          conn.Open();
          string cmdText = "SELECT 1 FROM ARESDatabase WHERE datname='temp'";
          using ( NpgsqlCommand cmd = new NpgsqlCommand( cmdText, conn ) )
          {
            connOk = true;
            bool exists = cmd.ExecuteScalar() != null;

            CheckResults = "Good to go!";
            return DBState.OK;
          }
        }
        catch(Exception)
        {
          if ( connOk )
          {
            CheckResults = "The ARESDatabase does not exist.";
            return DBState.BadDb;
          }
          else
          { 
            CheckResults = "The database connection does not exist.";
            return DBState.BadConnection;
          }
          
        }
      }
    }

    public string CheckResults
    {
      get => _checkResults;
      set => this.RaiseAndSetIfChanged(ref _checkResults , value);
    }
  }
}
