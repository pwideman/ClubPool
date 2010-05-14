using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class RoleMap : IAutoMappingOverride<Role>
  {
    public void Override(AutoMapping<Role> mapping) {
      mapping.HasManyToMany<User>(x => x.Users)
        .AsBag()
        .Table("UsersRoles");
    }
  }
}
