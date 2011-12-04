using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.NHibernate;

namespace ClubPool.Core.Contracts
{
  public interface IMatchResultRepository : ILinqRepository<MatchResult>
  {
    IQueryable<MatchResult> GetMatchResultsForPlayerAndGameType(User player, GameType gameType);
  }
}
