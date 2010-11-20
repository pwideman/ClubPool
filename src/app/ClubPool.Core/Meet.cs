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
    protected IList<Match> matches;

    public virtual int Week { get; protected set; }
    public virtual Division Division { get; protected set; }
    public virtual bool IsComplete { get; set; }
    public virtual IEnumerable<Team> Teams { get { return new Team[2] { Team1, Team2 }; } }
    public virtual Team Team1 { get; protected set; }
    public virtual Team Team2 { get; protected set; }
    public virtual IEnumerable<Match> Matches { get { return matches; } }

    protected Meet() {
      InitMembers();
    }

    public Meet(Team team1, Team team2, int week) : this() {
      Check.Require(null != team1, "team1 cannot be null");
      Check.Require(null != team2, "team2 cannot be null");
      Check.Require(team1.Division == team2.Division, "teams must be in the same division");
      Check.Require(week >= 0, "week must be >= 0");

      Team1 = team1;
      Team2 = team2;
      Division = team1.Division;
      Week = week;

      createMatches();
    }

    protected void InitMembers() {
      matches = new List<Match>();
    }

    protected void createMatches() {
      foreach (var team1Player in Team1.Players) {
        foreach (var team2Player in Team2.Players) {
          AddMatch(new Match(this, team1Player, team2Player));
        }
      }
    }

    public virtual void AddMatch(Match match) {
      if (!(Teams.Where(t => t.Players.Contains(match.Player1)).Any() && Teams.Where(t => t.Players.Contains(match.Player2)).Any())) {
        throw new ArgumentException("all players in match must be members of the meet's teams", "match");
      }
      if (!matches.Contains(match)) {
        match.Meet = this;
        matches.Add(match);
      }
    }

    public virtual void RemoveMatch(Match match) {
      if (matches.Contains(match)) {
        matches.Remove(match);
        match.Meet = null;
      }
    }

    public virtual void RemoveAllMatches() {
      foreach (var match in matches) {
        match.Meet = null;
      }
      matches.Clear();
    }

    public virtual bool UserCanEnterMatchResults(User user) {
      if (null == user) {
        return false;
      }
      // first check to see if this user is a member of the teams in the meet
      if (Teams.Where(t => t.Players.Contains(user)).Any()) {
        return true;
      }
      // next, check for role access
      if (user.IsInRole(Roles.Administrators) ||
          user.IsInRole(Roles.Officers)) {
        return true;
      }
      // if we haven't met any of these criteria, return false
      return false;
    }

  }
}
