using System;
using System.Linq;

using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

using SharpArch.Core.DomainModel;
using SharpArch.Data.NHibernate.FluentNHibernate;

using ClubPool.Core;
using ClubPool.Data.NHibernateMaps.Conventions;

namespace ClubPool.Data.NHibernateMaps
{

  public class AutoPersistenceModelGenerator : IAutoPersistenceModelGenerator
  {
    public AutoPersistenceModel Generate() {
      return AutoMap.AssemblyOf<User>(new ClubPoolMappingConfiguration())
        .Conventions.Setup(GetConventions())
        .IgnoreBase<Entity>()
        .IgnoreBase(typeof(EntityWithTypedId<>))
        .UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();
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
  }
}
