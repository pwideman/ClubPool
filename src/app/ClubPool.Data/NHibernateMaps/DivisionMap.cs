using FluentNHibernate.Automapping;
using FluentNHibernate;
using FluentNHibernate.Automapping.Alterations;

using ClubPool.Core;

namespace ClubPool.Data.NHibernateMaps
{
  public class DivisionMap : IAutoMappingOverride<Division>
  {
    public void Override(AutoMapping<Division> mapping) {
      mapping.Id(x => x.Id);
      mapping.Map(x => x.StartingDate);
      mapping.Map(x => x.Name);
      mapping.References(x => x.Season);
    }
  }
}
