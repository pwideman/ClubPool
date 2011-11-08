using System;

using Castle.Windsor;
using Castle.MicroKernel.Registration;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;

using SharpArch.Core.PersistenceSupport;
using SharpArch.Core.CommonValidator;
using SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

using ClubPool.Framework.NHibernate;
using Specs.ClubPool.Data.TestDoubles;

namespace Specs
{
  public class ServiceLocatorInitializer
  {
    public static void Init() {
      IWindsorContainer container = new WindsorContainer();
      container.Register(Component.For<IValidator>().ImplementedBy<Validator>());
      container.Register(Component.For<IEntityDuplicateChecker>().ImplementedBy<EntityDuplicateCheckerStub>());
      container.Register(Component.For(typeof(ILinqRepository<>)).ImplementedBy(typeof(LinqRepository<>)));
      ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
    }
  }
}
