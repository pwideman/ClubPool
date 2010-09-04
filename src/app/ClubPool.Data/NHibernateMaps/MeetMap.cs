using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class MeetMap : IAutoMappingOverride<Meet>
  {
    public void Override(AutoMapping<Meet> mapping) {
      mapping.Id(x => x.Id);
      mapping.Map(x => x.IsComplete);
      mapping.Map(x => x.Week);
      mapping.References(x => x.Division);
      mapping.HasManyToMany<Team>(x => x.Teams)
        .Access.ReadOnlyPropertyThroughCamelCaseField()
        .AsBag()
        .Table("MeetsTeams");
    }
  }
}
