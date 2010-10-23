using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class MeetMap : IAutoMappingOverride<Meet>
  {
    public void Override(AutoMapping<Meet> mapping) {
      //mapping.Id(x => x.Id);
      //mapping.Map(x => x.IsComplete);
      //mapping.Map(x => x.Week);
      //mapping.References(x => x.Division).Cascade.All();
      //mapping.References(x => x.Team1);
      //mapping.References(x => x.Team2);
      mapping.IgnoreProperty(x => x.Teams);
      //mapping.HasMany<Team>(x => x.Matches)
      //  .AsBag()
      //  .Cascade.AllDeleteOrphan()
      //  .Access.ReadOnlyPropertyThroughCamelCaseField();
    }
  }
}
