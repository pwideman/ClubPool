using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public class Team : Entity, IEntityWithVersion
  {
    protected IList<User> players;

    [DomainSignature]
    public virtual string Name { get; set; }

    [DomainSignature]
    public virtual Division Division { get; set; }

    public virtual int Version { get; protected set; }
    
    public virtual IEnumerable<User> Players { get { return players; } }

    protected Team() {
      InitMembers();
    }

    public Team(string name, Division division):this() {
      Check.Require(!string.IsNullOrEmpty(name), "name cannot be null or empty");
      Check.Require(division != null, "division cannot be null");

      Name = name;
      Division = division;
    }

    protected virtual void InitMembers() {
      players = new List<User>();
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
        if (players.Contains(match.Winner)) {
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
                    where match.IsComplete && match.Players.Contains(player)
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
      var tempPlayers = players.ToArray();
      foreach (var player in tempPlayers) {
        RemovePlayer(player);
      }
    }

    public virtual void RemovePlayer(User player) {
      Check.Require(null != player, "player cannot be null");

      if (players.Contains(player)) {
        players.Remove(player);
        // remove the player from any incomplete matches
        var meets = Division.Meets.Where(m => m.Teams.Contains(this));
        foreach (var meet in meets) {
          var matches = meet.Matches.ToList();
          foreach (var match in matches) {
            if (match.Players.Contains(player) && !match.IsComplete) {
              meet.RemoveMatch(match);
            }
          }
        }
      }

    }

    public virtual void AddPlayer(User player) {
      Check.Require(null != player, "player cannot be null");

      if (!players.Contains(player)) {
        players.Add(player);
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
            if (meet.Matches.Where(m => m.Players.Contains(opponent)).Count() < players.Count) {
              meet.AddMatch(new Match(meet, player, opponent));
            }
          }
        }
      }
    }
  }
}
