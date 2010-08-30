using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;

using SharpArch.Core.PersistenceSupport.NHibernate;
using SharpArch.Data.NHibernate;
using SharpArch.Core.PersistenceSupport;

using ClubPool.Framework.NHibernate;

namespace ClubPool.Web.CastleWindsor
{
  public class GenericRepositoriesInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store) {
      container.Register(
          Add(typeof(IEntityDuplicateChecker), typeof(EntityDuplicateChecker)),
          Add(typeof(IRepository<>), typeof(Repository<>)),
          Add(typeof(INHibernateRepository<>), typeof(NHibernateRepository<>)),
          Add(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>)),
          Add(typeof(INHibernateRepositoryWithTypedId<,>), typeof(NHibernateRepositoryWithTypedId<,>)),
          Add(typeof(ILinqRepository<>), typeof(LinqRepository<>)),
          Add(typeof(ILinqRepositoryWithTypedId<,>), typeof(LinqRepositoryWithTypedId<,>)));
    }

    private IRegistration Add(Type service, Type implementation) {
      return Component.For(service).ImplementedBy(implementation);
    }
  }
}