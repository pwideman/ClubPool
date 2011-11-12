using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Models;

namespace ClubPool.Web.Infrastructure.EntityFramework
{
  public class Repository : IRepository
  {
    private Lazy<ClubPoolContext> dbContext;

    public Repository(Lazy<ClubPoolContext> dbContext) {
      this.dbContext = dbContext;
    }

    private DbContext DbContext { 
      get { 
        return dbContext.Value; 
      } 
    }

    public T Get<T>(int id) where T : Entity {
      return DbContext.Set<T>().Find(id);
    }

    public IQueryable<T> All<T>() where T : Entity {
      return DbContext.Set<T>();
    }

    public T SaveOrUpdate<T>(T entity) where T : Entity {
      if (null == entity) return null;
      if (entity.IsTransient()) {
        DbContext.Set<T>().Add(entity);
      }
      return entity;
    }

    public void Delete<T>(T entity) where T : Entity {
      DbContext.Set<T>().Remove(entity);
      DbContext.SaveChanges();
    }

    public void Refresh(Models.Entity entity) {
      DbContext.Entry(entity).Reload();
    }

    public void Commit() {
      DbContext.SaveChanges();
    }
  }
}