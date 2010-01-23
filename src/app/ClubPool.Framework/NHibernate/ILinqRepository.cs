using System;
using System.Linq;
using System.Linq.Expressions;

using SharpArch.Core.PersistenceSupport;

namespace ClubPool.Framework.NHibernate
{
  /// <summary>
  /// Provides a standard interface for repositories which are data-access mechanism agnostic.
  /// 
  /// Since nearly all of the domain objects you create will have a type of int Id, this 
  /// base ILinqRepository leverages this assumption.  If you want an entity with a type 
  /// other than int, such as string, then use <see cref="ILinqRepositoryWithTypedId{T, IdT}" />.
  /// </summary>
  public interface ILinqRepository<T> : ILinqRepositoryWithTypedId<T, int> { }

  public interface ILinqRepositoryWithTypedId<T, IdT>
  {
    /// <summary>
    /// Returns null if a row is not found matching the provided Id.
    /// </summary>
    T Get(IdT id);

    /// <summary>
    /// Returns all of the items of a given type.
    /// </summary>
    IQueryable<T> GetAll();

    /// <summary>
    /// Looks for zero or more instances using the provided expression.
    /// </summary>
    IQueryable<T> FindAll(Expression<Func<T, bool>> criteria);

    /// <summary>
    /// Looks for a single instance using the provided expression.
    /// </summary>
    /// <exception cref="NonUniqueResultException" />
    T FindOne(Expression<Func<T, bool>> criteria);

    /// <summary>
    /// For entities with automatatically generated Ids, such as identity, SaveOrUpdate may 
    /// be called when saving or updating an entity.
    /// 
    /// Updating also allows you to commit changes to a detached object.  More info may be found at:
    /// http://www.hibernate.org/hib_docs/nhibernate/html_single/#manipulatingdata-updating-detached
    /// </summary>
    T SaveOrUpdate(T entity);

    /// <summary>
    /// I'll let you guess what this does.
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Provides a handle to application wide DB activities such as committing any pending changes,
    /// beginning a transaction, rolling back a transaction, etc.
    /// </summary>
    IDbContext DbContext { get; }
  }
}
