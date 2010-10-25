using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Meets.ViewModels
{
  public class MeetViewModel
  {
    public int Id { get; set; }
    public int ScheduledWeek { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public IEnumerable<MatchViewModel> Matches { get; protected set; }

    public MeetViewModel() {
      Matches = new MatchViewModel[0];
    }

    public MeetViewModel(Meet meet) {
      Id = meet.Id;
      ScheduledWeek = meet.Week + 1;
      ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7);
      Team1Name = meet.Team1.Name;
      Team2Name = meet.Team2.Name;
      var matches = new List<MatchViewModel>();
      foreach (var match in meet.Matches) {
        matches.Add(new MatchViewModel(match));
      }
      Matches = matches;
    }

  }

  //public class TeamViewModel
  //{
  //  public TeamViewModel() {
  //    Players = new List<PlayerViewModel>();
  //  }

  //  public TeamViewModel(Team team) {
  //    Name = team.Name;
  //    Players = team.Players.Select(p => new PlayerViewModel(p)).ToList();
  //  }

  //  public string Name { get; set; }
  //  public IEnumerable<PlayerViewModel> Players { get; set; }
  //}

  //public class PlayerViewModel
  //{
  //  public PlayerViewModel() {
  //  }

  //  public PlayerViewModel(User player) {
  //    Name = player.FullName;
  //  }

  //  public string Name { get; set; }
  //}

  public class MatchViewModel
  {
    public bool IsComplete { get; set; }
    public string DatePlayed { get; set; }
    public IEnumerable<MatchResultViewModel> Results { get; protected set; }

    public MatchViewModel() {
    }

    public MatchViewModel(Match match) {
      var results = new List<MatchResultViewModel>();
      IsComplete = match.IsComplete;
      if (IsComplete) {
        DatePlayed = match.DatePlayed.ToShortDateString();
        foreach (var result in match.Results) {
          results.Add(new MatchResultViewModel(result));
        }
      }
      else {
        foreach (var player in match.Players) {
          var incompleteResult = new MatchResultViewModel(player, match.Meet.Teams.Where(t => t.Players.Contains(player)).First());
          results.Add(incompleteResult);
        }
      }
      Results = results;
    }
  }

  public class MatchResultViewModel
  {
    public string TeamName { get; set; }
    public string PlayerName { get; set; }
    public int Innings { get; set; }
    public int DefensiveShots { get; set; }
    public int Wins { get; set; }
    public bool Winner { get; set; }

    public MatchResultViewModel(User player, Team team) {
      // incomplete result
      PlayerName = player.FullName;
      TeamName = team.Name;
    }

    public MatchResultViewModel(MatchResult result)
      : this(result.Player, result.Match.Meet.Teams.Where(t => t.Players.Contains(result.Player)).First()) {

      Innings = result.Innings;
      DefensiveShots = result.DefensiveShots;
      Wins = result.Wins;
      Winner = result.Match.Winner == result.Player;
    }
  }

}
