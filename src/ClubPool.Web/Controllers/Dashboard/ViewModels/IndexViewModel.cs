using System;
using System.Collections.Generic;
using System.Linq;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controllers.Shared.ViewModels;

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

    public IndexViewModel(User user, Team team, IRepository repository) {
      UserIsAdmin = user.IsInRole(Roles.Administrators);
      UserFullName = user.FullName;
      SkillLevelCalculation = new SkillLevelCalculationViewModel(user, repository);
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
                    where match.Players.Where(p => p.Player == user).Any()
                    select match;
      if (matches.Any()) {
        results = new List<SeasonResultViewModel>();
        foreach (var match in matches) {
          var result = new SeasonResultViewModel() {
            Player = match.Players.Where(p => p.Player != user).First().Player.FullName,
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
      vm.TeamId = team.Id;
      vm.TeamName = team.Name;
      var teammate = team.Players.Where(p => p != user).Single();
      vm.TeammateName = teammate.FullName;
      vm.TeammateId = teammate.Id;
      var winsAndLosses = team.GetWinsAndLossesForPlayer(user);
      vm.PersonalRecord = GetRecordText(winsAndLosses[0], winsAndLosses[1]);
      winsAndLosses = team.GetWinsAndLosses();
      vm.TeamRecord = GetRecordText(winsAndLosses[0], winsAndLosses[1]);
      return vm;
    }

    private string GetRecordText(int wins, int losses) {
      var pct = GetWinPercentage(wins, losses);
      return string.Format("{0} - {1} ({2})", wins, losses, pct.ToString(".00"));
    }

    private double GetWinPercentage(int wins, int losses) {
      double pct = 0;
      var total = wins + losses;
      if (total > 0) {
        pct = (double)wins / (double)total;
      }
      return pct;
    }
  }

  public class StatsViewModel
  {
    public int SkillLevel { get; set; }
    public string PersonalRecord { get; set; }
    public string TeamRecord { get; set; }
    public string TeamName { get; set; }
    public int TeamId { get; set; }
    public string TeammateName { get; set; }
    public int TeammateId { get; set; }
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
      DatePlayed = string.Format("{0} {1}", match.DatePlayed.Value.ToShortDateString(), match.DatePlayed.Value.ToShortTimeString());
      var results = new List<MatchResultViewModel>();
      foreach (var result in match.Results) {
        var resultvm = new MatchResultViewModel(result);
        resultvm.Winner = match.Winner == result.Player;
        results.Add(resultvm);
      }
      Results = results;
    }
  }

  public class SeasonResultViewModel
  {
    public string Team { get; set; }
    public string Player { get; set; }
    public bool Win { get; set; }
  }

}
