using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

using ClubPool.Web.Services.Authentication;
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
      var viewModel = BuildCreateTeamViewModel(repository, division);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles=Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateTeamViewModel viewModel) {
      var division = repository.Get<Division>(viewModel.DivisionId);

      if (!ModelState.IsValid) {
        var newViewModel = BuildCreateTeamViewModel(repository, division, viewModel);
        return View(newViewModel);
      }

      // verify that the team name is not already used
      if (division.TeamNameIsInUse(viewModel.Name)) {
        ModelState.AddModelErrorFor<CreateTeamViewModel>(m => m.Name, "This name is already in use");
        var newViewModel = BuildCreateTeamViewModel(repository, division, viewModel);
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

    private CreateTeamViewModel BuildCreateTeamViewModel(IRepository repository, Division division, CreateTeamViewModel viewModel = null) {
      var model = new CreateTeamViewModel();
      model.DivisionId = division.Id;
      model.DivisionName = division.Name;

      // load available players
      model.Players = GetAvailablePlayers(division.Season);

      if (null != viewModel) {
        CopyTeamViewModel(viewModel, model);
      }
      return model;
    }

    private EditTeamViewModel BuildEditTeamViewModel(IRepository repository, Team team, EditTeamViewModel viewModel = null) {
      var model = new EditTeamViewModel();
      model.Id = team.Id;
      model.Version = team.EncodedVersion;
      model.Name = team.Name;

      var availablePlayers = GetAvailablePlayers(team.Division.Season);

      // get team players and merge with available players
      var teamPlayers = team.Players.Select(p => new PlayerViewModel {
        Id = p.Id,
        Name = p.FullName,
        Username = p.Username,
        Email = p.Email,
        IsSelected = viewModel == null
      }).ToList();
      // always add them to the top so they are most visible
      availablePlayers.InsertRange(0, teamPlayers);
      model.Players = availablePlayers;
      model.SchedulePriority = team.SchedulePriority;

      if (null != viewModel) {
        CopyTeamViewModel(viewModel, model);
      }

      return model;
    }

    private void CopyTeamViewModel(TeamViewModel source, TeamViewModel copy) {
      copy.Name = source.Name;
      copy.SchedulePriority = source.SchedulePriority;
      if (null != source.SelectedPlayers && source.SelectedPlayers.Length > 0) {
        foreach (var playerId in source.SelectedPlayers) {
          copy.Players.Single(p => p.Id == playerId).IsSelected = true;
        }
      }
    }

    private List<PlayerViewModel> GetAvailablePlayers(Season season) {
      var sql = "select * from clubpool.Users where IsApproved = 1 and id not in " +
        "(select distinct u.id from clubpool.Users u, clubpool.TeamsUsers tp, clubpool.Teams t, clubpool.Divisions d, clubpool.Seasons s where " +
        "u.Id = tp.User_Id and s.id in (select Season_Id from clubpool.Divisions where Id in " +
        "(select Division_Id from clubpool.Teams where id = tp.Team_Id)) and s.Id = @p0)" +
        "order by LastName, FirstName";
      var availablePlayers = repository.SqlQuery<User>(sql, season.Id).Select(u => new PlayerViewModel() {
        Id = u.Id,
        Name = u.FullName,
        Username = u.Username,
        Email = u.Email
      }).ToList();
      return availablePlayers;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult Edit(int id) {
      var team = repository.Get<Team>(id);
      if (null == team) {
        return HttpNotFound();
      }
      var viewModel = BuildEditTeamViewModel(repository, team);
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
        var newViewModel = BuildEditTeamViewModel(repository, team, viewModel);
        return View(newViewModel);
      }

      if (team.Name != viewModel.Name) {
        if (team.Division.TeamNameIsInUse(viewModel.Name)) {
          ModelState.AddModelErrorFor<EditTeamViewModel>(m => m.Name, "Name is already in use");
          var newViewModel = BuildEditTeamViewModel(repository, team, viewModel);
          return View(newViewModel);
        }
        team.Name = viewModel.Name;
      }

      team.SchedulePriority = viewModel.SchedulePriority;

      if (null != viewModel.SelectedPlayers && viewModel.SelectedPlayers.Length > 0) {
        UpdatePlayers(team, viewModel.SelectedPlayers);
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

    private void UpdatePlayers(Team team, int[] players) {
      var newPlayers = new List<User>();
      foreach (var playerId in players) {
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

    private void AddPlayer(Team team, User player) {
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

    [Authorize]
    public ActionResult Details(int id) {
      var user = repository.Get<User>(authService.GetCurrentPrincipal().UserId);
      var team = repository.Get<Team>(id);
      if (null == team) {
        return HttpNotFound();
      }
      var viewModel = BuildDetailsViewModel(team);
      viewModel.CanUpdateName = UserCanUpdateTeamName(user, team);
      return View(viewModel);
    }

    protected bool UserCanUpdateTeamName(User user, Team team) {
      return user.IsInRole(Roles.Administrators) || user.IsInRole(Roles.Officers) || team.Players.Contains(user);
    }

    private DetailsViewModel BuildDetailsViewModel(Team team) {
      var model = new DetailsViewModel();
      model.Id = team.Id;
      model.Name = team.Name;
      model.Record = GetRecord(team);
      var players = new List<DetailsPlayerViewModel>();
      model.Email = "";
      foreach (var player in team.Players) {
        players.Add(BuildDetailsPlayerViewModel(player));
        if (model.Email.Length > 0) {
          model.Email += ",";
        }
        model.Email += player.Email;
      }
      model.Players = players;
      model.Rank = CalculateRank(team);
      var teamMeets = team.Division.Meets.Where(m => m.Teams.Contains(team) && m.Matches.Where(match => match.IsComplete).Any())
                                         .OrderByDescending(m => m.Week);
      if (teamMeets.Any()) {
        model.HasSeasonResults = true;
        var seasonResults = new List<DetailsMatchViewModel>();
        foreach (var meet in teamMeets) {
          seasonResults.AddRange(BuildDetailsMatchViewModels(meet, team));
        }
        model.SeasonResults = seasonResults;
      }
      else {
        model.HasSeasonResults = false;
      }
      return model;
    }

    private string GetRecord(Team team) {
      var winsAndLosses = team.GetWinsAndLosses();
      var wins = winsAndLosses[0];
      var losses = winsAndLosses[1];
      var total = wins + losses;
      double winPct = 0;
      if (total > 0) {
        winPct = (double)wins / (double)total;
      }
      return string.Format("{0} - {1} ({2:.00})", wins, losses, winPct);
    }

    private string CalculateRank(Team team) {
      var division = team.Division;
      var q = division.Teams.OrderByDescending(t => t.GetWinPercentage()).Select((o, i) => new { Rank = i, Team = o }).ToArray();
      var rank = q.Where(o => o.Team == team).Select(o => o.Rank).Single();
      var tied = false;
      var myWinPct = team.GetWinPercentage();
      if (rank > 0) {
        while (rank > 0 && q[rank - 1].Team.GetWinPercentage() == myWinPct) {
          rank--;
        }
      }
      if (rank < (q.Length - 1)) {
        tied = (q[rank + 1].Team.GetWinPercentage() == myWinPct);
      }
      return (tied ? "T" : "") + (rank + 1).ToString();
    }

    private DetailsPlayerViewModel BuildDetailsPlayerViewModel(User player) {
      var model = new DetailsPlayerViewModel();
      model.Id = player.Id;
      model.Name = player.FullName;
      var skillLevel = player.SkillLevels.Where(sl => sl.GameType == GameType.EightBall).FirstOrDefault();
      if (null != skillLevel) {
        model.EightBallSkillLevel = skillLevel.Value;
      }
      return model;
    }

    private List<DetailsMatchViewModel> BuildDetailsMatchViewModels(Meet meet, Team team) {
      var opponentTeamName = meet.Teams.Where(t => t != team).Single().Name;
      var matches = new List<DetailsMatchViewModel>();
      foreach (var match in meet.Matches.Where(m => m.IsComplete)) {
        var matchViewModel = BuildDetailsMatchViewModel(match, team);
        matchViewModel.OpponentTeamName = opponentTeamName;
        matches.Add(matchViewModel);
      }
      return matches;
    }

    private DetailsMatchViewModel BuildDetailsMatchViewModel(Match match, Team team) {
      var model = new DetailsMatchViewModel();
      var teamPlayer = match.Players.Where(p => p.Team == team).Single().Player;
      model.TeamPlayerName = teamPlayer.FullName;
      var oppPlayer = match.Players.Where(p => p.Player != teamPlayer).Single().Player;
      model.OpponentPlayerName = oppPlayer.FullName;
      model.Win = match.Winner == teamPlayer;
      if (!match.IsForfeit) {
        var teamResult = match.Results.Where(r => r.Player == teamPlayer).Single();
        model.TeamPlayerWins = teamResult.Wins;
        var oppResult = match.Results.Where(r => r != teamResult).Single();
        model.OpponentPlayerWins = oppResult.Wins;
      }
      return model;
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
