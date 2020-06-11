using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ARESCore.Database.Management.Impl;

namespace ARESCore.Database.Tables
{
  public class GenericDbRepository<T> where T : class
  {
    internal AresContext _context;
    internal DbSet<T> _dbSet;

    public GenericDbRepository(AresContext context)
    {
      _context = context;
      _dbSet = _context.Set<T>();
    }

    public virtual void Insert(T entity)
    {
      _dbSet.Add( entity );
      _context.SaveChanges();
    }
  }
}
