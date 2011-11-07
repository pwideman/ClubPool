﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Web.Controllers.Attributes;
using ClubPool.Web.Controllers.CurrentSeason.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Services.Authentication;

namespace ClubPool.Web.Controllers.CurrentSeason
{
  public class CurrentSeasonController : BaseController
  {
    protected ISeasonRepository seasonRepository;
    protected IUserRepository userRepository;
    protected IAuthenticationService authService;

    public CurrentSeasonController(ISeasonRepository seasonRepo, IUserRepository userRepo, IAuthenticationService authService) {
      Check.Require(null != seasonRepo, "seasonRepo cannot be null");
      Check.Require(null != userRepo, "userRepo cannot be null");
      Check.Require(null != authService, "authService cannot be null");

      seasonRepository = seasonRepo;
      userRepository = userRepo;
      this.authService = authService;
    }

    [Authorize]
    [HttpGet]
    [Transaction]
    public ActionResult Schedule() {
      var user = userRepository.Get(authService.GetCurrentPrincipal().UserId);
      var season = seasonRepository.GetAll().Where(s => s.IsActive).Single();
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
    [Transaction]
    public ActionResult Standings() {
      var user = userRepository.Get(authService.GetCurrentPrincipal().UserId);
      var season = seasonRepository.GetAll().Where(s => s.IsActive).Single();
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
    [Transaction]
    public ActionResult DownloadAllPlayersStandings() {
      var season = seasonRepository.GetAll().Where(s => s.IsActive).Single();
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