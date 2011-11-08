using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class MatchPlayer : Entity
  {
    public virtual Match Match { get; set; }
    public virtual User Player { get; set; }
    public virtual Team Team { get; set; }

    protected MatchPlayer() {
    }

    public MatchPlayer(User player, Team team) {
      Arg.NotNull(player, "player");
      Arg.NotNull(team, "team");

      Player = player;
      Team = team;
    }
  }
}
