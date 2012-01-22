using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using ClubPool.Web.Models;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Standings
{
  public class StandingsController : BaseController
  {
    private IRepository repository;
    private IAuthenticationService authService;

    public StandingsController(IRepository repo, IAuthenticationService authService) {
      Arg.NotNull(repo, "repo");
      Arg.NotNull(authService, "authService");

      repository = repo;
      this.authService = authService;
    }

    [Authorize]
    [HttpGet]
    public ActionResult Index() {
      var user = repository.Get<User>(authService.GetCurrentPrincipal().UserId);
      var season = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null == season) {
        return ErrorView("There is no current season");
      }
      else {
        var viewModel = CreateSeasonStandingsViewModel(season, user);
        return View(viewModel);
      }
    }

    private SeasonStandingsViewModel CreateSeasonStandingsViewModel(Season season, User userToHighlight) {
      var model = new SeasonStandingsViewModel();
      model.Name = season.Name;
      if (season.Divisions.Any()) {
        model.HasDivisions = true;
        var divisions = new List<DivisionStandingsViewModel>();
        foreach (var division in season.Divisions) {
          divisions.Add(CreateDivisionStandingsViewModel(division, userToHighlight));
        }
        model.Divisions = divisions;
        var players = new List<PlayerStandingsViewModel>();
        foreach (var division in divisions) {
          if (null != division.Players && division.Players.Any()) {
            players.AddRange(division.Players);
          }
        }
        model.AllPlayers = players.OrderByDescending(p => p.WinPercentage).ThenByDescending(p => p.Wins).ThenByDescending(p => p.SkillLevel);
      }
      else {
        model.HasDivisions = false;
      }
      return model;
    }

    public DivisionStandingsViewModel CreateDivisionStandingsViewModel(Division division, User userToHighlight) {
      var model = new DivisionStandingsViewModel();
      model.Id = division.Id;
      model.Name = division.Name;
      model.HasTeams = false;
      model.HasPlayers = false;
      if (division.Teams.Any()) {
        model.HasTeams = true;
        var players = new List<PlayerStandingsViewModel>();
        var teams = new List<TeamStandingsViewModel>();
        foreach (var team in division.Teams) {
          var teamvm = CreateTeamStandingsViewModel(team, userToHighlight);
          if (null != teamvm.Player1) {
            players.Add(teamvm.Player1);
          }
          if (null != teamvm.Player2) {
            players.Add(teamvm.Player2);
          }
          teams.Add(teamvm);
        }
        model.Teams = RankStandingsList(teams);
        if (players.Any()) {
          model.Players = RankStandingsList(players);
          model.HasPlayers = true;
        }
      }
      return model;
    }

    private TeamStandingsViewModel CreateTeamStandingsViewModel(Team team, User userToHighlight) {
      var model = new TeamStandingsViewModel();
      model.Id = team.Id;
      model.Name = team.Name;
      if (null != userToHighlight) {
        model.Highlight = team.Players.Contains(userToHighlight);
      }
      var winsAndLosses = team.GetWinsAndLosses();
      model.Wins = winsAndLosses[0];
      model.Losses = winsAndLosses[1];
      model.WinPercentage = CalculateWinPercentage(model.Wins, model.Losses);
      if (team.Players.Any()) {
        var players = team.Players.ToArray();
        if (players.Length > 0) {
          model.Player1 = CreatePlayerStandingsViewModel(players[0], team, userToHighlight);
          if (players.Length > 1) {
            model.Player2 = CreatePlayerStandingsViewModel(players[1], team, userToHighlight);
          }
        }
      }
      return model;
    }

    private PlayerStandingsViewModel CreatePlayerStandingsViewModel(User player, Team team, User userToHighlight) {
      var model = new PlayerStandingsViewModel();
      model.Id = player.Id;
      model.Name = player.FullName;
      if (null != userToHighlight) {
        model.Highlight = player == userToHighlight;
      }
      var gameType = team.Division.Season.GameType;
      var slQuery = player.SkillLevels.Where(sl => sl.GameType == gameType);
      if (slQuery.Any()) {
        model.SkillLevel = slQuery.Single().Value;
      }
      else {
        model.SkillLevel = 0;
      }
      var winsAndLosses = team.GetWinsAndLossesForPlayer(player);
      model.Wins = winsAndLosses[0];
      model.Losses = winsAndLosses[1];
      model.WinPercentage = CalculateWinPercentage(model.Wins, model.Losses);
      return model;
    }

    private double CalculateWinPercentage(int wins, int losses) {
      return (wins + losses > 0) ? ((double)wins / (double)(wins + losses)) : 0;
    }

    private List<T> RankStandingsList<T>(List<T> list) where T : StandingsViewModelBase {
      var newlist = list.OrderByDescending(t => t.WinPercentage).ThenByDescending(t => t.Wins).ToList();
      var count = newlist.Count;
      var currentRank = 1;
      string rank = "1";
      double prevWP = -1;
      for (int i = 0; i < count; i++) {
        var item = newlist[i];
        if (i > 0) {
          if (item.WinPercentage == prevWP) {
            rank = "T" + currentRank.ToString();
            newlist[i - 1].Rank = rank;
          }
          else {
            currentRank = i + 1;
            rank = currentRank.ToString();
          }
        }
        item.Rank = rank;
        prevWP = item.WinPercentage;
      }
      return newlist;
    }

    [Authorize]
    [HttpGet]
    public ActionResult DownloadAllPlayersStandings() {
      var season = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null == season) {
        return ErrorView("There is no current season");
      }
      else {
        // TODO: Use a more optimized way to get this data, this works for now
        var viewModel = CreateSeasonStandingsViewModel(season, null);
        Response.AddHeader("Content-Disposition", "attachment;filename=" + season.Name + ".csv");
        StringBuilder csv = new StringBuilder("Rank,Name,Skill Level,Wins,Losses,Win %\n");
        int i = 1;
        foreach (var player in viewModel.AllPlayers) {
          csv.AppendFormat("{0},{1},{2},{3},{4},{5:0.00}\n", i++, player.Name, player.SkillLevel,
            player.Wins, player.Losses, player.WinPercentage);
        }
        return Content(csv.ToString(), "text/csv");
      }
    }

  }
}
