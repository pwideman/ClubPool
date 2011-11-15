using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Models;

namespace ClubPool.Web.Infrastructure.EntityFramework
{
  public class Repository : IRepository
  {
    private Lazy<DbContext> dbContext;

    public Repository(Lazy<DbContext> dbContext) {
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

    public IQueryable<T> SqlQuery<T>(string sql, params object[] parameters) where T : Entity {
      return DbContext.Set<T>().SqlQuery(sql, parameters).AsQueryable();
    }

    public T SaveOrUpdate<T>(T entity) where T : Entity {
      if (null == entity) return null;
      if (entity.IsTransient()) {
        DbContext.Set<T>().Add(entity);
      }
      else {
        DbContext.Entry(entity).State = System.Data.EntityState.Modified;
      }
      return entity;
    }

    public void Delete<T>(T entity) where T : Entity {
      DbContext.Set<T>().Remove(entity);
      SaveChanges();
    }

    public void Refresh(Models.Entity entity) {
      DbContext.Entry(entity).Reload();
    }

    public void SaveChanges() {
      try {
        DbContext.SaveChanges();
      }
      catch (DbUpdateConcurrencyException e) {
        throw new UpdateConcurrencyException(e.Message);
      }
    }
  }
}