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
using ClubPool.ApplicationServices.DomainManagement.Contracts;

namespace ClubPool.Web.Controllers.Teams
{
  public class TeamsController : BaseController
  {
    protected ITeamRepository teamRepository;
    protected IDivisionRepository divisionRepository;
    protected IUserRepository userRepository;
    protected ITeamManagementService teamManagementService;

    public TeamsController(ITeamRepository teamRepo,
      IDivisionRepository divisionRepo,
      IUserRepository userRepo,
      ITeamManagementService teamManagementService) {

      Check.Require(null != teamRepo, "teamRepo cannot be null");
      Check.Require(null != divisionRepo, "divisionRepo cannot be null");
      Check.Require(null != userRepo, "userRepo cannot be null");
      Check.Require(null != teamManagementService, "teamManagementService cannot be null");

      teamRepository = teamRepo;
      divisionRepository = divisionRepo;
      userRepository = userRepo;
      this.teamManagementService = teamManagementService;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Create(int divisionId) {
      var division = divisionRepository.Get(divisionId);
      var viewModel = new CreateTeamViewModel(userRepository, division);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles=Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateTeamViewModel viewModel) {
      var division = divisionRepository.Get(viewModel.DivisionId);
      if (!ValidateViewModel(viewModel)) {
        viewModel.ReInitialize(userRepository, division.Season);
        return View(viewModel);
      }

      // verify that the team name is not already used
      if (teamManagementService.TeamNameIsInUse(division, viewModel.Name)) {
        ModelState.AddModelErrorFor<CreateTeamViewModel>(m => m.Name, "This name is already in use");
        viewModel.ReInitialize(userRepository, division.Season);
        return View(viewModel);
      }

      var team = new Team(viewModel.Name, division);
      if (viewModel.Players.Any()) {
        foreach (var playerViewModel in viewModel.Players) {
          var player = userRepository.Get(playerViewModel.Id);
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
        return HttpNotFound();
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
        return HttpNotFound();
      }
      var viewModel = new EditTeamViewModel(userRepository, team);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditTeamViewModel viewModel) {
      var team = teamRepository.Get(viewModel.Id);

      if (!ValidateViewModel(viewModel)) {
        viewModel.ReInitialize(userRepository, team);
        return View(viewModel);
      }

      if (team.Name != viewModel.Name) {
        if (teamManagementService.TeamNameIsInUse(team.Division, viewModel.Name)) {
          ModelState.AddModelErrorFor<EditTeamViewModel>(m => m.Name, "Name is already in use");
          viewModel.ReInitialize(userRepository, team);
          return View(viewModel);
        }
        team.Name = viewModel.Name;
      }

      team.RemoveAllPlayers();
      if (null != viewModel.Players && viewModel.Players.Any()) {
        foreach (var playerViewModel in viewModel.Players) {
          var player = userRepository.Get(playerViewModel.Id);
          team.AddPlayer(player);
        }
      }

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was updated";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(team.Division.Season.Id));
    }
  }
}
