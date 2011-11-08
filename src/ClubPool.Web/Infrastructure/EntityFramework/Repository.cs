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
    private ClubPoolContext dbContext;

    public Repository(ClubPoolContext dbContext) {
      this.dbContext = dbContext;
    }

    public IDbContext DbContext { get { return dbContext; } }

    public T Get<T>(int id) where T : Entity {
      return dbContext.Set<T>().Find(id);
    }

    public IQueryable<T> All<T>() where T : Entity {
      return dbContext.Set<T>();
    }

    public T SaveOrUpdate<T>(T entity) where T : Entity {
      if (null == entity) return null;
      if (entity.IsTransient()) {
        dbContext.Set<T>().Add(entity);
      }
      return entity;
    }

    public void Delete<T>(T entity) where T : Entity {
      dbContext.Set<T>().Remove(entity);
      dbContext.SaveChanges();
    }

    public void Refresh(Models.Entity entity) {
      dbContext.Entry(entity).Reload();
    }
  }
}