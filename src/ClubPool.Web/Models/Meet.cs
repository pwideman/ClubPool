using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using ClubPool.Web.Infrastructure;
using ClubPool.Core;

namespace ClubPool.Web.Models
{
  public class Meet : Entity
  {
    protected IList<Match> matches;
    protected IList<Team> teams;

    public virtual int Week { get; protected set; }
    public virtual Division Division { get; protected set; }
    public virtual bool IsComplete { get; set; }
    public virtual IEnumerable<Team> Teams { get { return teams; } }
    public virtual IEnumerable<Match> Matches { get { return matches; } }

    protected Meet() {
      InitMembers();
    }

    public Meet(Team team1, Team team2, int week) : this() {
      Arg.NotNull(team1, "team1");
      Arg.NotNull(team2, "team2");
      Arg.Require(team1.Division == team2.Division, "teams must be in the same division");
      Arg.Require(week >= 0, "week must be >= 0");

      Division = team1.Division;
      Week = week;

      teams.Add(team1);
      teams.Add(team2);
      createMatches();
    }

    protected void InitMembers() {
      matches = new List<Match>();
      teams = new List<Team>();
    }

    protected void createMatches() {
      var team1 = teams[0];
      var team2 = teams[1];
      foreach (var team1Player in team1.Players) {
        foreach (var team2Player in team2.Players) {
          AddMatch(new Match(this, new MatchPlayer(team1Player, team1), new MatchPlayer(team2Player, team2)));
        }
      }
    }

    public virtual void AddMatch(Match match) {
      var allTeamPlayers = (from team in Teams
                            from player in team.Players
                            select player).ToList();
      if (match.Players.Where(p => !allTeamPlayers.Contains(p.Player)).Any()) {
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
