using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class TeamMap : IAutoMappingOverride<Team>
  {
    public void Override(AutoMapping<Team> mapping) {
      mapping.Id(x => x.Id);
      mapping.Map(x => x.Name);
      mapping.Version(x => x.Version);
      mapping.References(x => x.Division);
      mapping.HasManyToMany<User>(x => x.Players)
        .Access.ReadOnlyPropertyThroughCamelCaseField()
        .AsBag()
        .Table("TeamsPlayers");
    }

  }
}
