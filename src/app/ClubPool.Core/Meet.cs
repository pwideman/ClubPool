using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class Meet : Entity
  {
    protected Meet() {
    }

    public Meet(Team team1, Team team2, int week) {
      Check.Require(null != team1, "team1 cannot be null");
      Check.Require(null != team2, "team2 cannot be null");
      Check.Require(team1.Division == team2.Division, "teams must be in the same division");
      Check.Require(week >= 0, "week must be >= 0");

      Team1 = team1;
      Team2 = team2;
      Division = team1.Division;
      Week = week;
    }

    public virtual int Week { get; protected set; }

    public virtual Division Division { get; protected set; }

    public virtual bool IsComplete { get; set; }

    public virtual IEnumerable<Team> Teams { get { return new Team[2] { Team1, Team2 }; } }

    public virtual Team Team1 { get; protected set; }

    public virtual Team Team2 { get; protected set; }
  }
}
