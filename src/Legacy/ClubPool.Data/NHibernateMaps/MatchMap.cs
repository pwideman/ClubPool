using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class MatchMap : IAutoMappingOverride<Match>
  {
    public void Override(AutoMapping<Match> mapping) {
      //mapping.Id(x => x.Id);
      //mapping.Map(x => x.IsComplete);
      //mapping.References(x => x.Meet).Cascade.All();
      //mapping.References(x => x.Winner);
      //mapping.References(x => x.Player1);
      //mapping.References(x => x.Player2);
      //mapping.HasMany<Team>(x => x.Results)
      //  .AsBag()
      //  .Cascade.AllDeleteOrphan()
      //  .Access.ReadOnlyPropertyThroughCamelCaseField();
      //mapping.HasManyToMany<User>(x => x.Players)
      //  .Cascade.SaveUpdate()
      //  .Access.ReadOnlyPropertyThroughCamelCaseField()
      //  .AsBag()
      //  .Table("MatchesPlayers");
    }
  }
}
