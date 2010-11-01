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
    public PlayerViewModel Player1;
    public PlayerViewModel Player2;
    public int Id;

    public IncompleteMatchViewModel(Match match) {
      Id = match.Id;

      var team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player1)).First();
      Player1 = new PlayerViewModel(match.Player1, team);

      team = match.Meet.Teams.Where(t => t.Players.Contains(match.Player2)).First();
      Player2 = new PlayerViewModel(match.Player2, team);

      Player1.GamesToWin = CalculateGamesToWin(Player1.SkillLevel, Player2.SkillLevel);
      Player2.GamesToWin = CalculateGamesToWin(Player2.SkillLevel, Player1.SkillLevel);
    }

    private int CalculateGamesToWin(int skillLevel, int opponentSkillLevel) {
      int gtw = 0;
      int maxDifference = 1; // number of games to reduce skill level by
      // compute GTW
      if (0 == skillLevel || 0 == opponentSkillLevel) {
        gtw = 4;
      }
      else {
        int difference = 0;
        if (skillLevel > opponentSkillLevel) {
          if (opponentSkillLevel > 3) {
            difference = opponentSkillLevel - 3;
            if (difference > maxDifference) {
              difference = maxDifference;
            }
          }
        }
        else {
          if (skillLevel > 3) {
            difference = skillLevel - 3;
          }
          if (difference > maxDifference) {
            difference = maxDifference;
          }
        }
        gtw = skillLevel - difference;
      }
      return gtw;
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
    public int GamesToWin { get; set; }

    public PlayerViewModel() {
    }

    public PlayerViewModel(User player, Team team) {
      Name = player.FullName;
      TeamName = team.Name;

      var gameType = team.Division.Season.GameType;
      var slQuery = player.SkillLevels.Where(sl => sl.GameType == gameType);
      if (slQuery.Any()) {
        SkillLevel = slQuery.First().Value;
      }
      else {
        SkillLevel = 0;
      }

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
