﻿using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

using ClubPool.Web.Services.Authentication;
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
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateTeamViewModel viewModel) {
      var division = repository.Get<Division>(viewModel.DivisionId);

      if (!ModelState.IsValid) {
        var newViewModel = new CreateTeamViewModel(repository, division);
        newViewModel.Name = viewModel.Name;
        newViewModel.SchedulePriority = viewModel.SchedulePriority;
        if (null != viewModel.SelectedPlayers && viewModel.SelectedPlayers.Length > 0) {
          foreach (var playerId in viewModel.SelectedPlayers) {
            var playerModel = newViewModel.Players.Single(p => p.Id == playerId);
            playerModel.IsSelected = true;
          }
        }
        return View(newViewModel);
      }

      // verify that the team name is not already used
      if (division.TeamNameIsInUse(viewModel.Name)) {
        ModelState.AddModelErrorFor<CreateTeamViewModel>(m => m.Name, "This name is already in use");
        var newViewModel = new CreateTeamViewModel(repository, division);
        newViewModel.Name = viewModel.Name;
        newViewModel.SchedulePriority = viewModel.SchedulePriority;
        if (null != viewModel.SelectedPlayers && viewModel.SelectedPlayers.Length > 0) {
          foreach (var playerId in viewModel.SelectedPlayers) {
            var playerModel = newViewModel.Players.Single(p => p.Id == playerId);
            playerModel.IsSelected = true;
          }
        }
        return View(newViewModel);
      }

      var team = new Team(viewModel.Name, division);
      team.SchedulePriority = viewModel.SchedulePriority;

      if (null != viewModel.SelectedPlayers && viewModel.SelectedPlayers.Length > 0) {
        foreach (var playerId in viewModel.SelectedPlayers) {
          var player = repository.Get<User>(playerId);
          team.AddPlayer(player);
        }
      }
      repository.SaveOrUpdate(team);
      repository.SaveChanges();
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was created successfully";
      return RedirectToAction("View", "Seasons", new { id = division.Season.Id });
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id) {
      var team = repository.Get<Team>(id);
      if (null == team) {
        return HttpNotFound();
      }
      var division = team.Division;
      var completedMatchesQuery = from meet in division.Meets
                                  from match in meet.Matches
                                  where meet.Teams.Contains(team) && match.IsComplete
                                  select match;
      if (completedMatchesQuery.Any()) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The team has completed matches, so it cannot be deleted";
      }
      else {
        DeleteTeam(team);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was deleted";
      }
      return RedirectToAction("View", "Seasons", new { id = division.Season.Id });
    }

    private void DeleteTeam(Team team) {
      var division = team.Division;
      var meets = division.Meets.Where(m => m.Teams.Contains(team)).ToList();
      foreach (var meet in meets) {
        division.Meets.Remove(meet);
        repository.Delete(meet);
      }
      division.Teams.Remove(team);
      repository.Delete(team);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
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
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditTeamViewModel viewModel) {
      var team = repository.Get<Team>(viewModel.Id);

      if (null == team) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The team you were editing was deleted by another user";
        return RedirectToAction("Index", "Seasons");
      }

      if (viewModel.Version != team.EncodedVersion) {
        return EditRedirectForConcurrency(viewModel.Id);
      }
      
      if (!ModelState.IsValid) {
        var newViewModel = new EditTeamViewModel(repository, team, false);
        newViewModel.Name = viewModel.Name;
        newViewModel.SchedulePriority = viewModel.SchedulePriority;
        if (null != viewModel.SelectedPlayers && viewModel.SelectedPlayers.Length > 0) {
          foreach (var playerId in viewModel.SelectedPlayers) {
            var playerModel = newViewModel.Players.Single(p => p.Id == playerId);
            playerModel.IsSelected = true;
          }
        }
        return View(newViewModel);
      }

      if (team.Name != viewModel.Name) {
        if (team.Division.TeamNameIsInUse(viewModel.Name)) {
          ModelState.AddModelErrorFor<EditTeamViewModel>(m => m.Name, "Name is already in use");
          var newViewModel = new EditTeamViewModel(repository, team, false);
          newViewModel.Name = viewModel.Name;
          newViewModel.SchedulePriority = viewModel.SchedulePriority;
          if (null != viewModel.SelectedPlayers && viewModel.SelectedPlayers.Length > 0) {
            foreach (var playerId in viewModel.SelectedPlayers) {
              var playerModel = newViewModel.Players.Single(p => p.Id == playerId);
              playerModel.IsSelected = true;
            }
          }
          return View(newViewModel);
        }
        team.Name = viewModel.Name;
      }

      team.SchedulePriority = viewModel.SchedulePriority;

      if (null != viewModel.SelectedPlayers && viewModel.SelectedPlayers.Length > 0) {
        var newPlayers = new List<User>();
        foreach (var playerId in viewModel.SelectedPlayers) {
          var player = repository.Get<User>(playerId);
          newPlayers.Add(player);
        }
        // first remove all players that aren't in the view model's players list
        var teamPlayers = team.Players.ToList();
        foreach (var teamPlayer in teamPlayers) {
          if (!newPlayers.Contains(teamPlayer)) {
            RemovePlayer(team, teamPlayer);
          }
        }

        // now add all new players to the team
        foreach (var newPlayer in newPlayers) {
          if (!team.Players.Contains(newPlayer)) {
            AddPlayer(team, newPlayer);
          }
        }
      }
      else {
        RemoveAllPlayers(team);
      }

      try {
        repository.SaveChanges();
      }
      catch (UpdateConcurrencyException) {
        return EditRedirectForConcurrency(viewModel.Id);
      }
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The team was updated";
      return RedirectToAction("View", "Seasons", new { id = team.Division.Season.Id });
    }

    public virtual void AddPlayer(Team team, User player) {
      if (!team.Players.Contains(player)) {
        team.Players.Add(player);
        // add this player to meets
        var meets = team.Division.Meets.Where(m => m.Teams.Contains(team));
        foreach (var meet in meets) {
          var opposingTeam = meet.Teams.Where(t => t.Id != team.Id).First();
          foreach (var opponent in opposingTeam.Players) {
            // loop through each player on the opposing team and see if they do not
            // already have a match against each player on this team. If not, add
            // a new match for the new player vs. opponent. We must do this check
            // because it's possible that some matches were played ahead of time
            // and one of the players in the match was removed from their team and
            // replaced by another player. In this case, the completed match stands.
            if (meet.Matches.Where(m => m.Players.Where(p => p.Player == opponent).Any()).Count() < team.Players.Count) {
              meet.AddMatch(new Match(meet, new MatchPlayer(player, team), new MatchPlayer(opponent, opposingTeam)));
              repository.SaveOrUpdate(meet);
            }
          }
        }
      }
    }

    private void RemoveAllPlayers(Team team) {
      var tempPlayers = team.Players.ToArray();
      foreach (var player in tempPlayers) {
        RemovePlayer(team, player);
      }
    }

    private void RemovePlayer(Team team, User player) {
      if (team.Players.Contains(player)) {
        team.Players.Remove(player);
        // remove the player from any incomplete matches
        var meets = team.Division.Meets.Where(m => m.Teams.Contains(team));
        foreach (var meet in meets) {
          var matches = meet.Matches.ToList();
          foreach (var match in matches) {
            if (match.Players.Where(p => p.Player == player).Any() && !match.IsComplete) {
              repository.Delete(match);
              meet.RemoveMatch(match);
            }
          }
        }
      }
    }

    private ActionResult EditRedirectForConcurrency(int id) {
      TempData[GlobalViewDataProperty.PageErrorMessage] =
        "This team was updated by another user while you were viewing this page. Enter your changes again.";
      return RedirectToAction("Edit", new { id = id });
    }

    [Authorize]
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UpdateName(UpdateNameViewModel viewModel) {
      if (!ModelState.IsValid) {
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
