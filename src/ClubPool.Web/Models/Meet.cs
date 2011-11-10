using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class Meet : Entity
  {
    public int Week { get; protected set; }
    [Required]
    public virtual Division Division { get; protected set; }
    public bool IsComplete { get; set; }
    public virtual ICollection<Team> Teams { get; private set; }
    public virtual ICollection<Match> Matches { get; private set; }

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

      Teams.Add(team1);
      Teams.Add(team2);
    }

    protected void InitMembers() {
      Matches = new HashSet<Match>();
      Teams = new HashSet<Team>();
    }

    public void CreateMatches() {
      var team1 = Teams.ElementAt(0);
      var team2 = Teams.ElementAt(1);
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
      if (!Matches.Contains(match)) {
        match.Meet = this;
        Matches.Add(match);
      }
    }

    public virtual void RemoveMatch(Match match) {
      if (Matches.Contains(match)) {
        Matches.Remove(match);
        match.Meet = null;
      }
    }

    public virtual void RemoveAllMatches() {
      foreach (var match in Matches) {
        match.Meet = null;
      }
      Matches.Clear();
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
