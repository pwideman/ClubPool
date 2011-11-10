using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class Team : VersionedEntity
  {
    [Required]
    public string Name { get; set; }
    // Division should be required, but SqlServerCe complains about a cyclical reference
    //[Required]
    public virtual Division Division { get; set; }
    public int SchedulePriority { get; set; }
    public virtual ICollection<User> Players { get; private set; }

    protected Team() {
      InitMembers();
    }

    public Team(string name, Division division):this() {
      Arg.NotNull(name, "name");
      Arg.NotNull(division, "division");

      Name = name;
      Division = division;
    }

    private void InitMembers() {
      Players = new HashSet<User>();
    }

    public virtual int[] GetWinsAndLosses() {
      var matches = from meet in Division.Meets
                    where meet.Teams.Contains(this)
                    from match in meet.Matches
                    where match.IsComplete
                    select match;
      int wins = 0;
      int losses = 0;
      foreach (var match in matches) {
        if (Players.Contains(match.Winner)) {
          wins++;
        }
        else {
          losses++;
        }
      }
      return new int[2] { wins, losses };
    }

    public virtual int[] GetWinsAndLossesForPlayer(User player) {
      var matches = from meet in Division.Meets
                    where meet.Teams.Contains(this)
                    from match in meet.Matches
                    where match.IsComplete && match.Players.Where(p => p.Player == player).Any()
                    select match;
      int wins = 0;
      int losses = 0;
      foreach (var match in matches) {
        if (match.Winner == player) {
          wins++;
        }
        else {
          losses++;
        }
      }
      return new int[2] { wins, losses };
    }

    public virtual double GetWinPercentage() {
      var winsAndLosses = GetWinsAndLosses();
      var wins = winsAndLosses[0];
      var losses = winsAndLosses[1];
      var total = wins + losses;
      if (total > 0) {
        return (double)wins / (double)total;
      }
      else {
        return 0;
      }
    }

    public virtual void RemoveAllPlayers() {
      var tempPlayers = Players.ToArray();
      foreach (var player in tempPlayers) {
        RemovePlayer(player);
      }
    }

    public virtual void RemovePlayer(User player) {
      Arg.NotNull(player, "player");

      if (Players.Contains(player)) {
        Players.Remove(player);
        // remove the player from any incomplete matches
        var meets = Division.Meets.Where(m => m.Teams.Contains(this));
        foreach (var meet in meets) {
          var matches = meet.Matches.ToList();
          foreach (var match in matches) {
            if (match.Players.Where(p => p.Player == player).Any() && !match.IsComplete) {
              meet.RemoveMatch(match);
            }
          }
        }
      }

    }

    public virtual void AddPlayer(User player) {
      Arg.NotNull(player, "player");

      if (!Players.Contains(player)) {
        Players.Add(player);
        // add this player to meets
        var meets = Division.Meets.Where(m => m.Teams.Contains(this));
        foreach (var meet in meets) {
          var opposingTeam = meet.Teams.Where(t => t != this).First();
          foreach (var opponent in opposingTeam.Players) {
            // loop through each player on the opposing team and see if they do not
            // already have a match against each player on this team. If not, add
            // a new match for the new player vs. opponent. We must do this check
            // because it's possible that some matches were played ahead of time
            // and one of the players in the match was removed from their team and
            // replaced by another player. In this case, the completed match stands.
            if (meet.Matches.Where(m => m.Players.Where(p => p.Player == opponent).Any()).Count() < Players.Count) {
              meet.AddMatch(new Match(meet, new MatchPlayer(player, this), new MatchPlayer(opponent, opposingTeam)));
            }
          }
        }
      }
    }
  }
}
