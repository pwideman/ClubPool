using System;
using System.Linq;
using System.Collections.Generic;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class MatchResult : Entity, IEntityWithVersion
  {
    protected MatchResult() {
    }

    public MatchResult(Match match, User player, int innings, int wins) {
      Check.Require(null != match, "match cannot be null");
      Check.Require(null != player, "player cannot be null");
      Check.Require(innings >= 0, "innings must be >= 0");
      Check.Require(wins >= 0, "wins must be >= 0");

      Player = player;
      Innings = innings;
      Wins = wins;
      Match = match;
    }

    public virtual Match Match { get; set; }
    public virtual User Player { get; set; }
    public virtual int Innings { get; set; }
    public virtual int Wins { get; set; }
    public virtual int Version { get; set; }
  }
}
