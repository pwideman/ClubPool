using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Core.Queries
{
  public static class SeasonQueries
  {
    public static IQueryable<Season> WithName(this IQueryable<Season> seasons, string name) {
      return seasons.Where(s => s.Name.Equals(name));
    }

    public static IQueryable<Season> WhereActive(this IQueryable<Season> seasons) {
      return seasons.Where(s => s.IsActive);
    }

    public static IQueryable<Season> WhereInactive(this IQueryable<Season> seasons) {
      return seasons.Where(s => !s.IsActive);
    }

  }
}
