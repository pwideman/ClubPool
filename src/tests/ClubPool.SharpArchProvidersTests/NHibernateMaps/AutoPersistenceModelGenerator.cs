using System;
using System.Linq;

using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;

using SharpArch.Core.DomainModel;
using SharpArch.Data.NHibernate.FluentNHibernate;

using ClubPool.SharpArchProviders.Domain;

using Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions;

namespace Tests.ClubPool.SharpArchProviders.NHibernateMaps
{

  public class AutoPersistenceModelGenerator : IAutoPersistenceModelGenerator
  {

    #region IAutoPersistenceModelGenerator Members

    public AutoPersistenceModel Generate() {
      var mappings = new AutoPersistenceModel();
      mappings.AddEntityAssembly(typeof(User).Assembly).Where(GetAutoMappingFilter);
      mappings.Conventions.Setup(GetConventions());
      mappings.Setup(GetSetup());
      mappings.IgnoreBase<Entity>();
      mappings.IgnoreBase(typeof(EntityWithTypedId<>));
      mappings.UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();

      return mappings;

    }

    #endregion

    private Action<AutoMappingExpressions> GetSetup() {
      return c => {
        c.FindIdentity = type => type.Name == "Id";
      };
    }

    private Action<IConventionFinder> GetConventions() {
      return c => {
        c.Add<Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions.ForeignKeyConvention>();
        c.Add<Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions.HasManyConvention>();
        c.Add<Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions.HasManyToManyConvention>();
        c.Add<Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions.ManyToManyTableNameConvention>();
        c.Add<Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions.PrimaryKeyConvention>();
        c.Add<Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions.ReferenceConvention>();
        c.Add<Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions.TableNameConvention>();
      };
    }

    /// <summary>
    /// Provides a filter for only including types which inherit from the IEntityWithTypedId interface.
    /// </summary>

    private bool GetAutoMappingFilter(Type t) {
      return t.GetInterfaces().Any(x =>
                                   x.IsGenericType &&
                                   x.GetGenericTypeDefinition() == typeof(IEntityWithTypedId<>));
    }
  }
}
