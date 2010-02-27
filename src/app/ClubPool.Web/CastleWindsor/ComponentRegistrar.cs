using System.Web.Security;

using Castle.Windsor;
using Castle.MicroKernel.Registration;

using SharpArch.Core.PersistenceSupport.NHibernate;
using SharpArch.Data.NHibernate;
using SharpArch.Core.PersistenceSupport;
using SharpArch.Web.Castle;
using SharpArch.Core.CommonValidator;
using SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

using ClubPool.Web.Security;
using ClubPool.Framework.NHibernate;

namespace ClubPool.Web.CastleWindsor
{
  public class ComponentRegistrar
  {
    public static void AddComponentsTo(IWindsorContainer container) {
      AddGenericRepositoriesTo(container);
      AddCustomRepositoriesTo(container);
      AddWebSecurityServicesTo(container);
      AddApplicationServicesTo(container);

      container.AddComponent("validator",
          typeof(IValidator), typeof(Validator));
    }

    private static void AddWebSecurityServicesTo(IWindsorContainer container) {
      // add the MembershipProvider for the application,
      // which will be setup in web.config.
      container.Register(
        Component.For<MembershipProvider>()
        .Instance(Membership.Provider));
      container.Register(
        Component.For<RoleProvider>()
        .Instance(Roles.Provider));
    }

    private static void AddApplicationServicesTo(IWindsorContainer container) {
      container.Register(
          AllTypes.Pick()
          .FromAssemblyNamed("ClubPool.ApplicationServices")
          .WithService.FirstInterface());
    }

    private static void AddCustomRepositoriesTo(IWindsorContainer container) {
      container.Register(
          AllTypes.Pick()
          .FromAssemblyNamed("ClubPool.Data")
          .WithService.FirstNonGenericCoreInterface("ClubPool.Core"));
    }

    private static void AddGenericRepositoriesTo(IWindsorContainer container) {
      container.AddComponent("entityDuplicateChecker",
          typeof(IEntityDuplicateChecker), typeof(EntityDuplicateChecker));
      container.AddComponent("repositoryType",
          typeof(IRepository<>), typeof(Repository<>));
      container.AddComponent("nhibernateRepositoryType",
          typeof(INHibernateRepository<>), typeof(NHibernateRepository<>));
      container.AddComponent("repositoryWithTypedId",
          typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));
      container.AddComponent("nhibernateRepositoryWithTypedId",
          typeof(INHibernateRepositoryWithTypedId<,>), typeof(NHibernateRepositoryWithTypedId<,>));
      container.AddComponent("linqRepositoryType",
          typeof(ILinqRepository<>), typeof(LinqRepository<>));
      container.AddComponent("linqRepositoryWithTypedId",
          typeof(ILinqRepositoryWithTypedId<,>), typeof(LinqRepositoryWithTypedId<,>));
    }
  }
}
