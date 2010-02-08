using System;

using Castle.Windsor;
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
      container.AddComponent("validator",
          typeof(IValidator), typeof(Validator));
      container.AddComponent("entityDuplicateChecker",
          typeof(IEntityDuplicateChecker), typeof(EntityDuplicateCheckerStub));
      container.AddComponent("linqRepositoryType",
          typeof(ILinqRepository<>), typeof(LinqRepository<>));
      ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
    }
  }
}
