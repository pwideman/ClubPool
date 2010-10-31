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
    public IEnumerable<CompletedMatchViewModel> CompletedMatches { get; protected set; }
    public IEnumerable<IncompleteMatchViewModel> IncompleteMatches { get; protected set; }

    public MeetViewModel() {
      CompletedMatches = new CompletedMatchViewModel[0];
      IncompleteMatches = new IncompleteMatchViewModel[0];
    }

    public MeetViewModel(Meet meet) {
      Id = meet.Id;
      ScheduledWeek = meet.Week + 1;
      ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7);
      Team1Name = meet.Team1.Name;
      Team2Name = meet.Team2.Name;
      var completedMatches = new List<CompletedMatchViewModel>();
      var incompleteMatches = new List<IncompleteMatchViewModel>();
      foreach (var match in meet.Matches) {
        if (match.IsComplete) {
          completedMatches.Add(new CompletedMatchViewModel(match));
        }
        else {
          incompleteMatches.Add(new IncompleteMatchViewModel(match));
        }
      }
      CompletedMatches = completedMatches;
      IncompleteMatches = incompleteMatches;
    }

  }

  public class IncompleteMatchViewModel
  {
    public IEnumerable<PlayerViewModel> Players;

    public IncompleteMatchViewModel(Match match) {
      var players = new List<PlayerViewModel>();
      foreach (var player in match.Players) {
        var team = match.Meet.Teams.Where(t => t.Players.Contains(player)).First();
        players.Add(new PlayerViewModel(player, team));
      }
      Players = players;
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

  public class PlayerViewModel
  {
    public string Name { get; set; }
    public int SkillLevel { get; set; }
    public string TeamName { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinPercentage { get; set; }
    public int Ranking { get; set; }

    public PlayerViewModel() {
    }

    public PlayerViewModel(User player, Team team) {
      Name = player.FullName;
      TeamName = team.Name;
      SkillLevel = 5;
      Wins = 8;
      Losses = 3;
      WinPercentage = (double)Wins/(double)(Wins+Losses);
      Ranking = 1;
    }
  }

  public class CompletedMatchViewModel
  {
    public string DatePlayed { get; set; }
    public IEnumerable<MatchResultViewModel> Results { get; protected set; }

    public CompletedMatchViewModel() {
    }

    public CompletedMatchViewModel(Match match) {
      var results = new List<MatchResultViewModel>();
      DatePlayed = match.DatePlayed.ToShortDateString();
      foreach (var result in match.Results) {
        results.Add(new MatchResultViewModel(result));
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

    public MatchResultViewModel(MatchResult result) {
      var team = result.Match.Meet.Teams.Where(t => t.Players.Contains(result.Player)).First();

      PlayerName = result.Player.FullName;
      TeamName = team.Name;
      Innings = result.Innings;
      DefensiveShots = result.DefensiveShots;
      Wins = result.Wins;
      Winner = result.Match.Winner == result.Player;
    }
  }

}
