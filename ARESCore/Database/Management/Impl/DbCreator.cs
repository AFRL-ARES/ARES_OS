using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Npgsql;

namespace ARESCore.Database.Management.Impl
{
  internal class DbCreator : IDbCreator
  {
    private AresContext _context;

    public DbCreator( AresContext context )
    {
      _context = context;
    }

    public void Create()
    {
     

      CreateAllTables();
    }

    private void CreateAllTables()
    {
      
      CreateMachineTable();
      CreatePlannerTable();
    }

    private void CreatePlannerTable()
    {
/*      _context.Planners.Add( new Database.Tables.PlannerEntity() { Id = Guid.NewGuid(), TimeStamp = DateTime.Now } );
      _context.SaveChanges();*/
    }


    private void CreateMachineTable()
    {
//      _context.MachineStates.Add(new Database.Tables.MachineStateEntity() { Id = Guid.NewGuid(), TimeStamp = DateTime.Now });
//      _context.SaveChanges();
    }
  }
}