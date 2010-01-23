using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using SharpArch.Data.NHibernate.FluentNHibernate;

using ClubPool.SharpArchProviders.Domain;

namespace Tests.SharpArchProviders.NHibernateMaps
{
  public class UserMap : IAutoMappingOverride<User>
  {
    public void Override(AutoMapping<User> mapping) {
      mapping.HasManyToMany<Role>(x => x.Roles)
        .Inverse()
        .AsBag();
    }
  }
}
