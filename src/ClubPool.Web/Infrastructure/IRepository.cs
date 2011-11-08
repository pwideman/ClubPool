using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Web.Models;

namespace ClubPool.Web.Infrastructure
{
  public interface IRepository
  {
    IDbContext DbContext { get; }
    T Get<T>(int id) where T : Entity;
    IQueryable<T> All<T>() where T : Entity;
    T SaveOrUpdate<T>(T entity) where T : Entity;
    void Delete<T>(T entity) where T : Entity;
    void Refresh(Entity entity);
  }
}
