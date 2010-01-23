using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Rhino.Mocks;

using SharpArch.Core;
using SharpArch.Core.PersistenceSupport;
using SharpArch.Core.DomainModel;
using SharpArch.Testing;

using ClubPool.Framework.NHibernate;

namespace Tests.ClubPool.SharpArchProviders.TestDoubles
{
  public class MockDbContext : IDbContext
  {
    #region IDbContext Members

    public IDisposable BeginTransaction() {
      return MockRepository.GenerateMock<IDisposable>();
    }

    public void CommitChanges() {
      // do nothing
    }

    public void CommitTransaction() {
      // do nothing
    }

    public void RollbackTransaction() {
      // do nothing
    }

    #endregion
  }

  public class MockRepository<T> : MockRepositoryWithTypedId<T, int>, ILinqRepository<T> where T : Entity
  {
    public MockRepository() : this(null) { }
    public MockRepository(IList<T> data) : base(data) { }
  }

  public class MockRepositoryWithTypedId<T, IdT> : ILinqRepositoryWithTypedId<T, IdT> where T : EntityWithTypedId<IdT>
  {
    protected IList<T> data { get; set; }

    protected IDbContext dbContext;

    public MockRepositoryWithTypedId() : this(null) { }

    public MockRepositoryWithTypedId(IList<T> data) {
      if (null == data) {
        this.data = new List<T>();
      }
      else {
        this.data = data;
      }
      dbContext = new MockDbContext();
    }

    public IDbContext DbContext { get { return dbContext; } }

    public void Delete(T entity) {
      data.Remove(entity);
    }

    public IQueryable<T> FindAll(Expression<Func<T, bool>> criteria) {
      return data.AsQueryable().Where(criteria);
    }

    public T FindOne(Expression<Func<T, bool>> criteria) {
      var one = data.AsQueryable().Where(criteria);
      if (one.Any()) {
        return one.First();
      }
      else {
        return default(T);
      }
    }

    public T Get(IdT id) {
      return data.AsQueryable().SingleOrDefault(t => t.Id.Equals(id));
    }

    public IQueryable<T> GetAll() {
      return data.AsQueryable();
    }

    public T SaveOrUpdate(T entity) {
      Check.Require(!(entity is IHasAssignedId<IdT>),
          "For better clarity and reliability, Entities with an assigned Id must call Save or Update");
      // first, see if the entity is already in the dataset
      var existingEntity = data.AsQueryable().SingleOrDefault(t => t.Equals(entity));
      if (null != existingEntity) {
        if (!ReferenceEquals(entity, existingEntity)) {
          // since we have no way of copying entity's properties to existingEntity,
          // replace it instead
          data.Remove(existingEntity);
          data.Add(entity);
        }
        return entity;
      }
      else if (entity.IsTransient()) {
        // The entity is transient but it could still have the same
        // domain signature as another entity, so find out
        existingEntity = data.AsQueryable().SingleOrDefault(t => t.HasSameObjectSignatureAs(entity));
        if (null != existingEntity) {
          // If we reach this point then the entity to save is transient, but it has the same
          // domain signature as an existing entity. I'm not real sure what to do in
          // this case - I don't know if this could possibly be a valid situation or
          // if it indicates an error. For now I'm going to assume it is an error
          throw new ArgumentException(
            "Entity is transient but has same domain signature as an existing entity, cannot save or update");
        }
        else {
          // new entity, save it
          var id = FindNextId();
          entity.SetIdTo<IdT>(id);
          data.Add(entity);
          return entity;
        }
      }
      else {
        // if we reach this point, that means the entity is not transient but
        // it does not exist in our dataset. Since we've already disallowed
        // entities with assigned Ids, I don't think this should ever happen
        throw new ArgumentException(
          "Entity is not transient but does not exist in dataset, cannot save or update");
      }
    }

    private IdT FindNextId() {
      // for some reason we have to explicitly convert the IList<T> over to
      // IList<EntityWithTypedId<IdT>>, even though we've declared T as
      // type EntityWithTypedId<IdT>. Our FindNextId() extension method
      // is not available on IList<T>
      IList<EntityWithTypedId<IdT>> myData = new List<EntityWithTypedId<IdT>>();
      foreach (var tEntity in data) {
        myData.Add(tEntity as EntityWithTypedId<IdT>);
      }
      IdT id = myData.FindNextId();
      return id;
    }
  }
}
