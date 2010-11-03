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

  //public class IncompleteMatchViewModel
  //{
  //  public PlayerViewModel Player1;
  //  public PlayerViewModel Player2;
  //  public int Id;

  //  public IncompleteMatchViewModel(Match match) {
  //    Id = match.Id;

  //    var team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player1)).First();
  //    Player1 = new PlayerViewModel(match.Player1, team);

  //    team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player2)).First();
  //    Player2 = new PlayerViewModel(match.Player2, team);

  //    Player1.GamesToWin = CalculateGamesToWin(Player1.SkillLevel, Player2.SkillLevel);
  //    Player2.GamesToWin = CalculateGamesToWin(Player2.SkillLevel, Player1.SkillLevel);
  //  }

  //  private int CalculateGamesToWin(int skillLevel, int opponentSkillLevel) {
  //    int gtw = 0;
  //    int maxDifference = 1; // number of games to reduce skill level by
  //    // compute GTW
  //    if (0 == skillLevel || 0 == opponentSkillLevel) {
  //      gtw = 4;
  //    }
  //    else {
  //      int difference = 0;
  //      if (skillLevel > opponentSkillLevel) {
  //        if (opponentSkillLevel > 3) {
  //          difference = opponentSkillLevel - 3;
  //          if (difference > maxDifference) {
  //            difference = maxDifference;
  //          }
  //        }
  //      }
  //      else {
  //        if (skillLevel > 3) {
  //          difference = skillLevel - 3;
  //        }
  //        if (difference > maxDifference) {
  //          difference = maxDifference;
  //        }
  //      }
  //      gtw = skillLevel - difference;
  //    }
  //    return gtw;
  //  }

  //}

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

  public class MatchPlayerViewModel : PlayerViewModel
  {
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinPercentage { get; set; }
    public MatchResultViewModel Result { get; set; }

    public MatchPlayerViewModel(User player, Team team)
      : base(player, team) {

      var completedMatches = from meet in team.Division.Schedule
                             where meet.Teams.Contains(team)
                             from match in meet.Matches
                             where match.Players.Contains(player) && match.IsComplete
                             select match;

      foreach (var match in completedMatches) {
        if (match.Winner == player) {
          Wins++;
        }
        else {
          Losses++;
        }
      }
      if (Wins + Losses > 0) {
        WinPercentage = (double)Wins / (double)(Wins + Losses);
      }
    }
  }

  public class MatchViewModel
  {
    public string DatePlayed { get; set; }
    public MatchPlayerViewModel Player1 { get; protected set; }
    public MatchPlayerViewModel Player2 { get; protected set; }
    public bool IsComplete { get; set; }
    public int Id { get; set; }

    public MatchViewModel() {
    }

    public MatchViewModel(Match match) {
      Id = match.Id;
      IsComplete = match.IsComplete;
      DatePlayed = match.DatePlayed.ToShortDateString();
      var team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player1)).First();
      Player1 = new MatchPlayerViewModel(match.Player1, team);
      team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player2)).First();
      Player2 = new MatchPlayerViewModel(match.Player2, team);

      if (match.IsComplete) {
        Player1.Result = new MatchResultViewModel(match.Results.Where(r => r.Player == match.Player1).First());
        Player2.Result = new MatchResultViewModel(match.Results.Where(r => r.Player == match.Player2).First());
      }
    }
  }

  public class MatchResultViewModel
  {
    public int Innings { get; set; }
    public int DefensiveShots { get; set; }
    public int Wins { get; set; }
    public bool Winner { get; set; }

    public MatchResultViewModel(MatchResult result) {
      Innings = result.Innings;
      DefensiveShots = result.DefensiveShots;
      Wins = result.Wins;
      Winner = result.Match.Winner == result.Player;
    }
  }

}
