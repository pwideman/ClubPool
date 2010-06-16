using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class RoleMap : IAutoMappingOverride<Role>
  {
    public void Override(AutoMapping<Role> mapping) {
      mapping.Id(x => x.Id);
      mapping.Map(x => x.Name);
      mapping.HasManyToMany<User>(x => x.Users)
        .Access.ReadOnlyPropertyThroughCamelCaseField()
        .Inverse()
        .AsBag()
        .Table("UsersRoles");
    }
  }
}
