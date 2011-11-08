using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class MatchResultMap : IAutoMappingOverride<MatchResult>
  {
    public void Override(AutoMapping<MatchResult> mapping) {
      //mapping.Id(x => x.Id);
      //mapping.References(x => x.Match).Cascade.All();
      //mapping.References(x => x.Player);
      //mapping.Map(x => x.Innings);
      //mapping.Map(x => x.Wins);
      //mapping.Version(x => x.Version);
    }
  }
}
