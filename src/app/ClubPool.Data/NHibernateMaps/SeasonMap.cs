using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class SeasonMap : IAutoMappingOverride<Season>
  {
    public void Override(AutoMapping<Season> mapping) {
      mapping.Id(x => x.Id);
      mapping.Map(x => x.IsActive);
      mapping.Map(x => x.Name);
      mapping.HasMany<Division>(x => x.Divisions)
        .AsBag()
        .Access.ReadOnlyPropertyThroughCamelCaseField();
    }
  }
}
