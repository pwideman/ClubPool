﻿using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Core;
using ClubPool.Core.Contracts;

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
    public SkillLevelCalculationViewModel SkillLevelCalculation { get; set; }

    public IndexViewModel(User user, Team team, IMatchResultRepository matchResultRepository) {
      UserIsAdmin = user.IsInRole(Roles.Administrators);
      UserFullName = user.FullName;
      SkillLevelCalculation = new SkillLevelCalculationViewModel(user, matchResultRepository);
      if (null != team) {
        CurrentSeasonStats = GetCurrentSeasonStatsViewModel(user, team);
        HasCurrentSeasonStats = CurrentSeasonStats != null;
        LastMeetStats = GetLastMeetStats(user, team);
        HasLastMeetStats = LastMeetStats != null;
        SeasonResults = GetSeasonResults(user, team);
        HasSeasonResults = SeasonResults != null;
      }
    }

    protected IEnumerable<SeasonResultViewModel> GetSeasonResults(User user, Team team) {
      List<SeasonResultViewModel> results = null;
      var matches = from m in team.Division.Meets
                    where m.Teams.Contains(team) && m.IsComplete
                    orderby m.Week descending
                    from match in m.Matches
                    where match.Players.Contains(user)
                    select match;
      if (matches.Any()) {
        results = new List<SeasonResultViewModel>();
        foreach (var match in matches) {
          var result = new SeasonResultViewModel() {
            Player = match.Players.Where(p => p != user).First().FullName,
            Team = match.Meet.Teams.Where(t => t != team).First().Name,
            Win = match.Winner == user
          };
          results.Add(result);
        }
      }
      return results;
    }

    protected LastMeetViewModel GetLastMeetStats(User user, Team team) {
      LastMeetViewModel viewModel = null;
      var meet = (from m in team.Division.Meets
                  where m.Teams.Contains(team) && m.IsComplete
                  orderby m.Week descending
                  select m).FirstOrDefault();
      if (null != meet) {
        viewModel = new LastMeetViewModel(meet, team);
      }
      return viewModel;
    }

    protected StatsViewModel GetCurrentSeasonStatsViewModel(User user, Team team) {
      var vm = new StatsViewModel();
      var skillLevel = user.SkillLevels.Where(sl => sl.GameType == team.Division.Season.GameType).FirstOrDefault();
      if (null != skillLevel) {
        vm.SkillLevel = skillLevel.Value;
      }
      vm.TeamName = team.Name;
      vm.Teammate = team.Players.Where(p => p != user).Single().FullName;
      var winsAndLosses = team.GetWinsAndLossesForPlayer(user);
      var pct = (double)winsAndLosses[0] / (double)(winsAndLosses[0] + winsAndLosses[1]);
      vm.PersonalRecord = string.Format("{0} - {1} ({2})", winsAndLosses[0], winsAndLosses[1], pct.ToString(".00"));
      winsAndLosses = team.GetWinsAndLosses();
      pct = (double)winsAndLosses[0] / (double)(winsAndLosses[0] + winsAndLosses[1]);
      vm.TeamRecord = string.Format("{0} - {1} ({2})", winsAndLosses[0], winsAndLosses[1], pct.ToString(".00"));
      return vm;
    }
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
    public IEnumerable<MatchResultViewModel> Results { get; set; }

    public LastMatchViewModel(Match match) {
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

    public MatchResultViewModel() {
    }

    public MatchResultViewModel(MatchResult result) {
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

  public class SkillLevelCalculationViewModel
  {
    public IEnumerable<SkillLevelMatchResultViewModel> SkillLevelMatchResults { get; set; }
    public int TotalInnings { get; set; }
    public int TotalDefensiveShots { get; set; }
    public int TotalNetInnings { get; set; }
    public int TotalWins { get; set; }
    public int TotalCulledInnings { get; set; }
    public int TotalCulledDefensiveShots { get; set; }
    public int TotalCulledNetInnings { get; set; }
    public int TotalCulledWins { get; set; }
    public bool HasSkillLevel { get; set; }

    public SkillLevelCalculationViewModel(User player, IMatchResultRepository matchResultRepository) {
      var matchResults = player.GetMatchResultsUsedInSkillLevelCalculation(GameType.EightBall, matchResultRepository);
      if (null != matchResults && matchResults.Count > 0) {
        HasSkillLevel = true;
        var culledMatchResults = player.CullTopMatchResults(matchResults);
        var results = new List<SkillLevelMatchResultViewModel>();
        foreach (var result in matchResults) {
          var resultvm = new SkillLevelMatchResultViewModel(result, player);
          results.Add(resultvm);
          if (culledMatchResults.Contains(result)) {
            resultvm.Included = true;
            TotalCulledInnings += resultvm.Innings;
            TotalCulledDefensiveShots += resultvm.DefensiveShots;
            TotalCulledNetInnings += resultvm.NetInnings;
            TotalCulledWins += resultvm.Wins;
          }
          TotalInnings += resultvm.Innings;
          TotalDefensiveShots += resultvm.DefensiveShots;
          TotalNetInnings += resultvm.NetInnings;
          TotalWins += resultvm.Wins;
        }
        SkillLevelMatchResults = results;
      }
    }
  }

  public class SkillLevelMatchResultViewModel : MatchResultViewModel
  {
    public string Team { get; set; }
    public string Date { get; set; }
    public bool Included { get; set; }
    public int NetInnings { get; set; }

    public SkillLevelMatchResultViewModel(MatchResult matchResult, User player) : base(matchResult) {
      var match = matchResult.Match;
      Date = string.Format("{0} {1}", match.DatePlayed.ToShortDateString(), match.DatePlayed.ToShortTimeString());
      Team = match.Meet.Teams.Where(t => !t.Players.Contains(player)).Single().Name;
      NetInnings = Innings - DefensiveShots;
      // for this we display our opponent
      Player = match.Players.Where(p => p != player).Single().FullName;

    }
  }
}
