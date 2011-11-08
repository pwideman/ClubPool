using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class UserMap : IAutoMappingOverride<User>
  {
    public void Override(AutoMapping<User> mapping) {
      //mapping.Id(x => x.Id);
      mapping.HasManyToMany<Role>(x => x.Roles)
        .Cascade.SaveUpdate()
        .Access.ReadOnlyPropertyThroughCamelCaseField()
        .AsBag()
        .Table("UsersRoles");
      mapping.IgnoreProperty(x => x.FullName);
      //mapping.Map(x => x.IsApproved);
      //mapping.Map(x => x.IsLocked);
      //mapping.Map(x => x.Email);
      //mapping.Map(x => x.Username);
      //mapping.Map(x => x.FirstName);
      //mapping.Map(x => x.LastName);
      //mapping.Map(x => x.Password);
      //mapping.Map(x => x.PasswordSalt);
      //mapping.Version(x => x.Version);
    }
  }
}
