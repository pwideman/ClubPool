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

    public virtual void AddPlayer(User player) {
      Arg.NotNull(player, "player");

      if (!Players.Contains(player)) {
        Players.Add(player);
      }
    }
  }
}
