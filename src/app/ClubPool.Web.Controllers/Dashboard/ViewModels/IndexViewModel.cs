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
    public LastMeetViewModel LastMeetStats { get; set; }
    public bool HasSeasonResults { get; set; }
    public IEnumerable<SeasonResultViewModel> SeasonResults { get; set; }
  }

  public class StatsViewModel
  {
    public int SkillLevel { get; set; }
    public string PersonalRecord { get; set; }
    public string TeamRecord { get; set; }
    public string TeamName { get; set; }
    public string Teammate { get; set; }
  }

  public class LastMeetViewModel
  {
    public string OpponentTeam { get; set; }
    public IEnumerable<LastMatchViewModel> Matches { get; set; }

    public LastMeetViewModel(Meet meet, Team team) {
      OpponentTeam = meet.Teams.Where(t => t != team).Single().Name;
      var matches = new List<LastMatchViewModel>();
      foreach (var match in meet.Matches) {
        var matchvm = new LastMatchViewModel(match);
        matches.Add(matchvm);
      }
      Matches = matches;
    }
  }

  public class LastMatchViewModel
  {
    public string DatePlayed { get; set; }
    public IEnumerable<LastMatchResultViewModel> Results { get; set; }

    public LastMatchViewModel(Match match) {
      DatePlayed = string.Format("{0} {1}", match.DatePlayed.ToShortDateString(), match.DatePlayed.ToShortTimeString());
      var results = new List<LastMatchResultViewModel>();
      foreach (var result in match.Results) {
        var resultvm = new LastMatchResultViewModel(result);
        resultvm.Winner = match.Winner == result.Player;
        results.Add(resultvm);
      }
      Results = results;
    }
  }

  public class LastMatchResultViewModel
  {
    public string Player { get; set; }
    public int Innings { get; set; }
    public int DefensiveShots { get; set; }
    public int Wins { get; set; }
    public bool Winner { get; set; }

    public LastMatchResultViewModel(MatchResult result) {
      Player = result.Player.FullName;
      Innings = result.Innings;
      DefensiveShots = result.DefensiveShots;
      Wins = result.Wins;
    }
  }

  public class SeasonResultViewModel
  {
    public string Team { get; set; }
    public string Player { get; set; }
    public bool Win { get; set; }
  }
}
