using System;
using System.Collections.Generic;
using System.Linq;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class MatchPlayer : Entity
  {
    public virtual Match Match { get; set; }
    public virtual User Player { get; set; }
    public virtual Team Team { get; set; }

    protected MatchPlayer() {
    }

    public MatchPlayer(User player, Team team) {
      Check.Require(null != player, "player cannot be null");
      Check.Require(null != team, "team cannot be null");

      Player = player;
      Team = team;
    }
  }
}
