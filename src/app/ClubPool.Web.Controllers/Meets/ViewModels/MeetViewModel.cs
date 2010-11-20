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
    public string ScheduledDate { get; set; }
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public IEnumerable<MatchViewModel> Matches { get; protected set; }

    public MeetViewModel() {
      Matches = new MatchViewModel[0];
    }

    public MeetViewModel(Meet meet) {
      Id = meet.Id;
      ScheduledWeek = meet.Week + 1;
      ScheduledDate = meet.Division.StartingDate.AddDays(meet.Week * 7).ToShortDateString();
      Team1Name = meet.Team1.Name;
      Team2Name = meet.Team2.Name;
      var matches = new List<MatchViewModel>();
      var early = true;
      foreach (var match in meet.Matches) {
        var matchViewModel = new MatchViewModel(match);
        if (early) {
          matchViewModel.TimeScheduled = "06:00 PM";
        }
        else {
          matchViewModel.TimeScheduled = "07:30 PM";
        }
        early = !early;
        matches.Add(matchViewModel);
      }
      Matches = matches;
    }

  }

  public class MatchPlayerViewModel : PlayerViewModel
  {
    public bool Winner { get; set; }
    public string Record { get; set; }
    public string Innings { get; set; }
    public string DefensiveShots { get; set; }
    public string Wins { get; set; }

    public MatchPlayerViewModel(User player, Match match) {
      Initialize(player, match.Meet.Division.Season.GameType);

      // initialize my results, if this match is complete
      if (match.IsComplete) {
        Winner = match.Winner == player;
        if (!match.IsForfeit) {
          var result = match.Results.Where(r => r.Player == player).First();
          Innings = result.Innings.ToString();
          DefensiveShots = result.DefensiveShots.ToString();
          Wins = result.Wins.ToString();
        }
      }

      // calculate my season record & win percentage
      var completedMatches = from meet in match.Meet.Division.Meets
                             where meet.IsComplete
                             from ma in meet.Matches
                             where ma.Player1 == player || ma.Player2 == player
                             select ma;

      int wins = 0;
      int losses = 0;
      double winPercentage = 0;

      foreach (var completedMatch in completedMatches) {
        if (completedMatch.Winner == player) {
          wins++;
        }
        else {
          losses++;
        }
      }
      if (wins + losses > 0) {
        winPercentage = (double)wins / (double)(wins + losses);
      }
      Record = string.Format("{0} - {1} ({2})", wins, losses, winPercentage.ToString(".00"));
    }
  }

  public class MatchViewModel
  {
    public string DatePlayed { get; set; }
    public string TimePlayed { get; set; }
    public string TimeScheduled { get; set; }
    public MatchPlayerViewModel Player1 { get; protected set; }
    public MatchPlayerViewModel Player2 { get; protected set; }
    public bool IsComplete { get; set; }
    public bool IsForfeit { get; set; }
    public int Id { get; set; }
    public string Status { get; set; }

    public MatchViewModel() {
    }

    public MatchViewModel(Match match) {
      Id = match.Id;
      IsComplete = match.IsComplete;
      IsForfeit = match.IsForfeit;
      Player1 = new MatchPlayerViewModel(match.Player1, match);
      Player2 = new MatchPlayerViewModel(match.Player2, match);
      if (match.IsComplete) {
        if (!match.IsForfeit) {
          DatePlayed = match.DatePlayed.ToShortDateString();
          TimePlayed = match.DatePlayed.ToShortTimeString();
          Status = string.Format("Played on {0} {1}", DatePlayed, TimePlayed);
        }
        else {
          Status = "Forfeited";
        }
      }
      else {
        Status = "Incomplete";
      }
    }
  }
}
