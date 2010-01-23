using System;
using System.Linq;

using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;

using SharpArch.Core.DomainModel;
using SharpArch.Data.NHibernate.FluentNHibernate;

using ClubPool.Core;
using ClubPool.Data.NHibernateMaps.Conventions;
using ClubPool.SharpArchProviders.Domain;

namespace ClubPool.Data.NHibernateMaps
{

  public class AutoPersistenceModelGenerator : IAutoPersistenceModelGenerator
  {

    #region IAutoPersistenceModelGenerator Members

    public AutoPersistenceModel Generate() {
      var mappings = new AutoPersistenceModel();
      mappings.AddEntityAssembly(typeof(Player).Assembly).Where(GetAutoMappingFilter);
      mappings.AddEntityAssembly(typeof(User).Assembly).Where(GetAutoMappingFilter);
      mappings.Conventions.Setup(GetConventions());
      mappings.Setup(GetSetup());
      mappings.IgnoreBase<Entity>();
      mappings.IgnoreBase(typeof(EntityWithTypedId<>));
      mappings.UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();
      mappings.UseOverridesFromAssemblyOf<User>();

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
        c.Add<ClubPool.Data.NHibernateMaps.Conventions.ForeignKeyConvention>();
        c.Add<ClubPool.Data.NHibernateMaps.Conventions.HasManyConvention>();
        c.Add<ClubPool.Data.NHibernateMaps.Conventions.HasManyToManyConvention>();
        c.Add<ClubPool.Data.NHibernateMaps.Conventions.ManyToManyTableNameConvention>();
        c.Add<ClubPool.Data.NHibernateMaps.Conventions.PrimaryKeyConvention>();
        c.Add<ClubPool.Data.NHibernateMaps.Conventions.ReferenceConvention>();
        c.Add<ClubPool.Data.NHibernateMaps.Conventions.TableNameConvention>();
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
