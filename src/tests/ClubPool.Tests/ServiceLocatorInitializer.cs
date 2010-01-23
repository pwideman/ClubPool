using System;

using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;

using SharpArch.Core.PersistenceSupport;
using SharpArch.Core.CommonValidator;
using SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

using ClubPool.Framework.NHibernate;
using Tests.ClubPool.Framework.NHibernate.TestDoubles;
using Tests.ClubPool.Data.TestDoubles;

namespace Tests
{
  public class ServiceLocatorInitializer
  {
    public static void Init(bool useMockRepository) {
      IWindsorContainer container = new WindsorContainer();
      container.AddComponent("validator",
          typeof(IValidator), typeof(Validator));
      container.AddComponent("entityDuplicateChecker",
          typeof(IEntityDuplicateChecker), typeof(EntityDuplicateCheckerStub));
      Type repositoryType = useMockRepository ? typeof(MockRepository<>) : typeof(LinqRepository<>);
      container.AddComponent("linqRepositoryType",
          typeof(ILinqRepository<>), repositoryType);
      ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
    }
  }
}
