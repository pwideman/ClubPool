using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Core;

namespace ClubPool.Web.Controllers.Dashboard.ViewModels
{
  public class IndexViewModel : ViewModelBase
  {
    public bool UserIsAdmin { get; set; }
    public string UserFullName { get; set; }
    public bool HasCurrentSeasonStats { get; set; }
    public StatsViewModel CurrentSeasonStats { get; set; }
    public bool HasLastMeetStats { get; set; }
    public MeetViewModel LastMeetStats { get; set; }
  }

  public class StatsViewModel
  {
    public int SkillLevel { get; set; }
    public string PersonalRecord { get; set; }
    public string TeamRecord { get; set; }
    public string TeamName { get; set; }
    public string Teammate { get; set; }
  }

  public class MeetViewModel
  {
    public string OpponentTeam { get; set; }
    public IEnumerable<MatchViewModel> Matches { get; set; }

    public MeetViewModel(Meet meet, Team team) {
      OpponentTeam = meet.Teams.Where(t => t != team).Single().Name;
      var matches = new List<MatchViewModel>();
      foreach (var match in meet.Matches) {
        var matchvm = new MatchViewModel(match);
        matches.Add(matchvm);
      }
      Matches = matches;
    }
}

  public class MatchViewModel
  {
    public string DatePlayed { get; set; }
    public IEnumerable<MatchResultViewModel> Results { get; set; }

    public MatchViewModel(Match match) {
      DatePlayed = string.Format("{0} {1}", match.DatePlayed.ToShortDateString(), match.DatePlayed.ToShortTimeString());
      var results = new List<MatchResultViewModel>();
      foreach (var result in match.Results) {
        var resultvm = new MatchResultViewModel(result);
        resultvm.Winner = match.Winner == result.Player;
        results.Add(resultvm);
      }
      Results = results;
    }
  }

  public class MatchResultViewModel
  {
    public string Player { get; set; }
    public int Innings { get; set; }
    public int DefensiveShots { get; set; }
    public int Wins { get; set; }
    public bool Winner { get; set; }

    public MatchResultViewModel(MatchResult result) {
      Player = result.Player.FullName;
      Innings = result.Innings;
      DefensiveShots = result.DefensiveShots;
      Wins = result.Wins;
    }
  }
}
