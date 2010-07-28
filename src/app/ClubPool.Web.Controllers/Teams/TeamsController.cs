using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers.Teams
{
  public class TeamsController : BaseController
  {
    protected ILinqRepository<Team> teamRepository;
    protected ILinqRepository<Division> divisionRepository;
    protected IUserRepository userRepository;

    public TeamsController(ILinqRepository<Team> teamRepo,
      ILinqRepository<Division> divisionRepo,
      IUserRepository userRepo) {

      Check.Require(null != teamRepo, "teamRepo cannot be null");
      Check.Require(null != divisionRepo, "divisionRepo cannot be null");
      Check.Require(null != userRepo, "userRepo cannot be null");

      teamRepository = teamRepo;
      divisionRepository = divisionRepo;
      userRepository = userRepo;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Create(int divisionId) {
      var division = divisionRepository.Get(divisionId);
      var viewModel = new TeamViewModel();
      viewModel.Division = new DivisionDto(division);
      viewModel.AvailablePlayers = userRepository.GetUnassignedUsersForSeason(division.Season).Select(u => new UserDto(u)).ToArray();
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles=Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Create(TeamViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }

      var division = divisionRepository.Get(viewModel.Division.Id);
      var team = new Team(viewModel.Name, division);
      if (null != viewModel.Players && viewModel.Players.Length > 0) {
        foreach (var playerDto in viewModel.Players) {
          var player = userRepository.Get(playerDto.Id);
          team.AddPlayer(player);
        }
      }
      teamRepository.SaveOrUpdate(team);

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was created successfully";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(division.Season.Id));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id) {
      var team = teamRepository.Get(id);
      if (null == team) {
        throw new HttpException((int)HttpStatusCode.NotFound, "The requested resource is not found");
      }
      if (!team.CanDelete()) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The team cannot be deleted";
      }
      else {
        teamRepository.Delete(team);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was deleted";
      }
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(team.Division.Season.Id));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Edit(int id) {
      var team = teamRepository.Get(id);
      if (null == team) {
        throw new HttpException((int)HttpStatusCode.NotFound, "The requested resource is not found");
      }
      var viewModel = new TeamViewModel();
      viewModel.Id = id;
      viewModel.Name = team.Name;
      viewModel.Players = team.Players.Select(p => new UserDto(p)).ToArray();
      viewModel.AvailablePlayers = userRepository.GetUnassignedUsersForSeason(team.Division.Season).Select(u => new UserDto(u)).ToArray();
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(TeamViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }

      var team = teamRepository.Get(viewModel.Id);
      team.Name = viewModel.Name;
      team.RemoveAllPlayers();
      if (null != viewModel.Players && viewModel.Players.Length > 0) {
        foreach (var playerDto in viewModel.Players) {
          var player = userRepository.Get(playerDto.Id);
          team.AddPlayer(player);
        }
      }
      teamRepository.SaveOrUpdate(team);

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was updated";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(team.Division.Season.Id));
    }
  }
}
