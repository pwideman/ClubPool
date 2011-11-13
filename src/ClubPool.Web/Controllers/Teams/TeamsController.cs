using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using xVal.ServerSide;

using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Attributes;
using ClubPool.Web.Controllers.Shared.ViewModels;
using ClubPool.Web.Controllers.Teams.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Teams
{
  public class TeamsController : BaseController
  {
    protected IRepository repository;
    protected IAuthenticationService authService;

    public TeamsController(IRepository repo,
      IAuthenticationService authService) {

      Arg.NotNull(repo, "repo");
      Arg.NotNull(authService, "authService");

      repository = repo;
      this.authService = authService;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    //[Transaction]
    public ActionResult Create(int divisionId) {
      var division = repository.Get<Division>(divisionId);
      if (null == division) {
        return HttpNotFound();
      }
      var viewModel = new CreateTeamViewModel(repository, division);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles=Roles.Administrators)]
    //[Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateTeamViewModel viewModel) {
      var division = repository.Get<Division>(viewModel.DivisionId);

      if (!ValidateViewModel(viewModel)) {
        viewModel.ReInitialize(repository, division.Season);
        return View(viewModel);
      }

      // verify that the team name is not already used
      if (division.TeamNameIsInUse(viewModel.Name)) {
        ModelState.AddModelErrorFor<CreateTeamViewModel>(m => m.Name, "This name is already in use");
        viewModel.ReInitialize(repository, division.Season);
        return View(viewModel);
      }

      var team = new Team(viewModel.Name, division);
      team.SchedulePriority = viewModel.SchedulePriority;

      if (viewModel.Players.Any()) {
        foreach (var playerViewModel in viewModel.Players) {
          var player = repository.Get<User>(playerViewModel.Id);
          team.AddPlayer(player);
        }
      }
      repository.SaveOrUpdate(team);
      repository.SaveChanges();
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was created successfully";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(division.Season.Id));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    //[Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id) {
      var team = repository.Get<Team>(id);
      if (null == team) {
        return HttpNotFound();
      }
      var division = team.Division;
      if (division.Meets.Any()) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The team cannot be deleted";
      }
      else {
        division.RemoveTeam(team);
        repository.Delete(team);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was deleted";
      }
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(division.Season.Id));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    //[Transaction]
    public ActionResult Edit(int id) {
      var team = repository.Get<Team>(id);
      if (null == team) {
        return HttpNotFound();
      }
      var viewModel = new EditTeamViewModel(repository, team);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    //[Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditTeamViewModel viewModel) {
      var team = repository.Get<Team>(viewModel.Id);

      if (null == team) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The team you were editing was deleted by another user";
        return this.RedirectToAction<Seasons.SeasonsController>(c => c.Index(null));
      }

      if (viewModel.Version != team.EncodedVersion) {
        return EditRedirectForConcurrency(viewModel.Id);
      }
      
      if (!ValidateViewModel(viewModel)) {
        viewModel.ReInitialize(repository, team);
        return View(viewModel);
      }

      if (team.Name != viewModel.Name) {
        if (team.Division.TeamNameIsInUse(viewModel.Name)) {
          ModelState.AddModelErrorFor<EditTeamViewModel>(m => m.Name, "Name is already in use");
          viewModel.ReInitialize(repository, team);
          return View(viewModel);
        }
        team.Name = viewModel.Name;
      }

      team.SchedulePriority = viewModel.SchedulePriority;

      if (null != viewModel.Players && viewModel.Players.Any()) {
        var newPlayers = new List<User>();
        foreach (var playerViewModel in viewModel.Players) {
          var player = repository.Get<User>(playerViewModel.Id);
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

      try {
        repository.SaveChanges();
      }
      catch (UpdateConcurrencyException) {
        return EditRedirectForConcurrency(viewModel.Id);
      }
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was updated";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(team.Division.Season.Id));
    }

    private ActionResult EditRedirectForConcurrency(int id) {
      TempData[GlobalViewDataProperty.PageErrorMessage] =
        "This team was updated by another user while you were viewing this page. Enter your changes again.";
      return this.RedirectToAction(c => c.Edit(id));
    }

    [Authorize]
    //[Transaction]
    public ActionResult Details(int id) {
      var user = repository.Get<User>(authService.GetCurrentPrincipal().UserId);
      var team = repository.Get<Team>(id);
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
    //[Transaction]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateName(UpdateNameViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return AjaxUpdate(false, "Invalid name");
      }
      var team = repository.Get<Team>(viewModel.Id);
      if (null == team) {
        return HttpNotFound();
      }
      var user = repository.Get<User>(authService.GetCurrentPrincipal().UserId);
      if (!UserCanUpdateTeamName(user, team)) {
        return AjaxUpdate(false, "You do not have permission to update this team's name");
      }
      if (team.Division.TeamNameIsInUse(viewModel.Name)) {
        return AjaxUpdate(false, string.Format("There is already a team named '{0}' in this team's division", viewModel.Name));
      }
      else {
        team.Name = viewModel.Name;
        try {
          repository.SaveChanges();
        }
        catch (UpdateConcurrencyException) {
          return AjaxUpdate(false, "The team was updated by another user while your request was pending, try again.");
        }
      }
      return AjaxUpdate();
    }


  }
}
