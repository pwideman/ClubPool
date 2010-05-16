using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class UserMap : IAutoMappingOverride<User>
  {
    public void Override(AutoMapping<User> mapping) {
      mapping.Id(x => x.Id);
      mapping.HasManyToMany<Role>(x => x.Roles)
        .Inverse()
        .AsBag()
        .Table("UsersRoles");
      mapping.IgnoreProperty(x => x.FullName);
      mapping.Map(x => x.IsApproved);
    }
  }
}
