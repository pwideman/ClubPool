using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.SharpArchProviders.Domain;

namespace ClubPool.SharpArchProviders.NHibernateMaps
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
