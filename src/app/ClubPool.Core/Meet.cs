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
    protected IList<Team> teams;

    protected Meet() {
      InitMembers();
    }

    public Meet(Team team1, Team team2, int week) {
      Check.Require(null != team1, "team1 cannot be null");
      Check.Require(null != team2, "team2 cannot be null");
      Check.Require(team1.Division == team2.Division, "teams must be in the same division");
      Check.Require(week >= 0, "week must be >= 0");

      InitMembers();
      teams.Add(team1);
      teams.Add(team2);
      Division = team1.Division;
      Week = week;
    }

    protected virtual void InitMembers() {
      teams = new List<Team>();
    }

    public virtual int Week { get; protected set; }

    public virtual Division Division { get; protected set; }

    public virtual bool IsComplete { get; set; }

    public virtual IEnumerable<Team> Teams { get { return teams; } }


  }
}
