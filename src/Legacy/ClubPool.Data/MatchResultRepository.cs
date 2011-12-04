using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;

namespace ClubPool.Data
{
  public class MatchResultRepository : LinqRepository<MatchResult>, IMatchResultRepository
  {
    public IQueryable<MatchResult> GetMatchResultsForPlayerAndGameType(User player, GameType gameType) {
      return from result in GetAll()
             where result.Player == player && result.Match.Meet.Division.Season.GameType == gameType
             select result;
    }

  }
}
