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

using ClubPool.Web.Infrastructure;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Attributes;
using ClubPool.Web.Controllers.Shared.ViewModels;
using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Web.Controllers.Extensions;

namespace ClubPool.Web.Controllers.Teams
{
  public class TeamsController : BaseController
  {
    protected ITeamRepository teamRepository;
    protected IDivisionRepository divisionRepository;
    protected IUserRepository userRepository;
    protected IAuthenticationService authService;

    public TeamsController(ITeamRepository teamRepo,
      IDivisionRepository divisionRepo,
      IUserRepository userRepo,
      IAuthenticationService authService) {

      Check.Require(null != teamRepo, "teamRepo cannot be null");
      Check.Require(null != divisionRepo, "divisionRepo cannot be null");
      Check.Require(null != userRepo, "userRepo cannot be null");
      Check.Require(null != authService, "authService cannot be null");

      teamRepository = teamRepo;
      divisionRepository = divisionRepo;
      userRepository = userRepo;
      this.authService = authService;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Create(int divisionId) {
      var division = divisionRepository.Get(divisionId);
      if (null == division) {
        return HttpNotFound();
      }
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
      if (division.TeamNameIsInUse(viewModel.Name)) {
        ModelState.AddModelErrorFor<CreateTeamViewModel>(m => m.Name, "This name is already in use");
        viewModel.ReInitialize(userRepository, division.Season);
        return View(viewModel);
      }

      var team = new Team(viewModel.Name, division);
      team.SchedulePriority = viewModel.SchedulePriority;

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
      var division = team.Division;
      if (division.Meets.Any()) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The team cannot be deleted";
      }
      else {
        division.RemoveTeam(team);
        teamRepository.Delete(team);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was deleted";
      }
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(division.Season.Id));
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

      if (null == team) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The team you were editing was deleted by another user";
        return this.RedirectToAction<Seasons.SeasonsController>(c => c.Index(null));
      }

      if (viewModel.Version != team.Version) {
        TempData[GlobalViewDataProperty.PageErrorMessage] =
          "This team was updated by another user while you were viewing this page. Enter your changes again.";
        return this.RedirectToAction(c => c.Edit(viewModel.Id));
      }
      
      if (!ValidateViewModel(viewModel)) {
        viewModel.ReInitialize(userRepository, team);
        return View(viewModel);
      }

      if (team.Name != viewModel.Name) {
        if (team.Division.TeamNameIsInUse(viewModel.Name)) {
          ModelState.AddModelErrorFor<EditTeamViewModel>(m => m.Name, "Name is already in use");
          viewModel.ReInitialize(userRepository, team);
          return View(viewModel);
        }
        team.Name = viewModel.Name;
      }

      team.SchedulePriority = viewModel.SchedulePriority;

      if (null != viewModel.Players && viewModel.Players.Any()) {
        var newPlayers = new List<User>();
        foreach (var playerViewModel in viewModel.Players) {
          var player = userRepository.Get(playerViewModel.Id);
          newPlayers.Add(player);
        }
        // first remove all players that aren't in the view model's players list
        var teamPlayers = team.Players.ToList();
        foreach (var teamPlayer in teamPlayers) {
          if (!newPlayers.Contains(teamPlayer)) {
            team.RemovePlayer(teamPlayer);
          }
        }
        // now add all new players to the team
        foreach (var newPlayer in newPlayers) {
          if (!team.Players.Contains(newPlayer)) {
            team.AddPlayer(newPlayer);
          }
        }
      }
      else {
        team.RemoveAllPlayers();
      }

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was updated";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(team.Division.Season.Id));
    }

    [Authorize]
    [Transaction]
    public ActionResult Details(int id) {
      var user = userRepository.Get(authService.GetCurrentPrincipal().UserId);
      var team = teamRepository.Get(id);
      if (null == team) {
        return HttpNotFound();
      }
      var viewModel = new DetailsViewModel(team);
      viewModel.CanUpdateName = UserCanUpdateTeamName(user, team);
      return View(viewModel);
    }

    protected bool UserCanUpdateTeamName(User user, Team team) {
      return user.IsInRole(Roles.Administrators) || user.IsInRole(Roles.Officers) || team.Players.Contains(user);
    }

    [Authorize]
    [Transaction]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateName(UpdateNameViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return AjaxUpdate(false, "Invalid name");
      }
      var team = teamRepository.Get(viewModel.Id);
      if (null == team) {
        return HttpNotFound();
      }
      var user = userRepository.Get(authService.GetCurrentPrincipal().UserId);
      if (!UserCanUpdateTeamName(user, team)) {
        return AjaxUpdate(false, "You do not have permission to update this team's name");
      }
      if (team.Division.TeamNameIsInUse(viewModel.Name)) {
        return AjaxUpdate(false, string.Format("There is already a team named '{0}' in this team's division", viewModel.Name));
      }
      else {
        team.Name = viewModel.Name;
      }
      return AjaxUpdate();
    }


  }
}
