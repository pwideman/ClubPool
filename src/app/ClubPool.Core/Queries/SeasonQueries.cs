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
  }
}
