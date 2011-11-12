using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using xVal.ServerSide;

using ClubPool.Framework.Validation;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Attributes;
using ClubPool.Web.Controllers.CurrentSeason.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.CurrentSeason
{
  public class CurrentSeasonController : BaseController
  {
    protected IRepository repository;
    protected IAuthenticationService authService;

    public CurrentSeasonController(IRepository repo, IAuthenticationService authService) {
      Arg.NotNull(repo, "repo");
      Arg.NotNull(authService, "authService");

      repository = repo;
      this.authService = authService;
    }

    [Authorize]
    [HttpGet]
    public ActionResult Schedule() {
      var user = repository.Get<User>(authService.GetCurrentPrincipal().UserId);
      var season = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null == season) {
        return ErrorView("There is no current season");
      }
      else {
        var viewModel = new CurrentSeasonScheduleViewModel(season, user);
        return View(viewModel);
      }
    }

    [Authorize]
    [HttpGet]
    public ActionResult Standings() {
      var user = repository.Get<User>(authService.GetCurrentPrincipal().UserId);
      var season = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null == season) {
        return ErrorView("There is no current season");
      }
      else {
        var viewModel = new CurrentSeasonStandingsViewModel(season, user);
        return View(viewModel);
      }
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
        var viewModel = new CurrentSeasonStandingsViewModel(season, null);
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
