using System.Collections.Specialized;
using System;
using System.Linq;
using System.Linq.Expressions;

using NHibernate.Linq;
using NHibernate;
using SharpArch.Data.NHibernate;

namespace ClubPool.Framework.NHibernate
{
  /// <summary>
  /// Since nearly all of the domain objects you create will have a type of int Id, this 
  /// most freqently used base LinqRepository leverages this assumption.  If you want an entity
  /// with a type other than int, such as string, then use 
  /// <see cref="LinqRepositoryWithTypedId{T, IdT}" />.
  /// </summary>
  public class LinqRepository<T> : LinqRepositoryWithTypedId<T, int>, ILinqRepository<T> { }

  /// <summary>
  /// Provides a fully loaded repository which may be created in a few ways including:
  /// * Direct instantiation; e.g., new LinqRepositoryWithTypedId<Customer, string>
  /// * Spring configuration; e.g., <object id="CustomerRepository" type="SharpArch.Data.NHibernate.LinqRepositoryWithTypedId<CustomerAlias, string>, SharpArch.Data" autowire="byName" />
  /// </summary>
  public class LinqRepositoryWithTypedId<T, IdT> : RepositoryWithTypedId<T, IdT>, ILinqRepositoryWithTypedId<T, IdT>
  {
    public new IQueryable<T> GetAll() {
      return Session.Query<T>();
    }

    public IQueryable<T> FindAll(Expression<Func<T, bool>> criteria) {
      return Session.Query<T>().Where(criteria);
    }

    public T FindOne(Expression<Func<T, bool>> criteria) {
      return Session.Query<T>().SingleOrDefault(criteria);
    }

    public void Refresh(T entity) {
      Session.Refresh(entity);
    }
  }
}
