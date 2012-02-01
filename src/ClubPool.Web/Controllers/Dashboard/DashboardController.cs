using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Dashboard.ViewModels;
using ClubPool.Web.Controllers.Dashboard.SidebarGadgets;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Dashboard
{
  public class DashboardController : BaseController, IRouteRegistrar
  {
    private IAuthenticationService authenticationService;
    private IRepository repository;

    public DashboardController(IAuthenticationService authSvc,
      IRepository repository) {

      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(repository, "repository");

      authenticationService = authSvc;
      this.repository = repository;
    }

    public void RegisterRoutes(System.Web.Routing.RouteCollection routes) {
      routes.MapRoute("dashboard", "dashboard/{id}", new { Controller = "Dashboard", Action = "Dashboard", id = UrlParameter.Optional });
    }

    [Authorize]
    public ActionResult Dashboard(int? id) {
      var principal = authenticationService.GetCurrentPrincipal();
      User user = null;
      if (id.HasValue) {
        if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
          user = repository.All<User>().Single(u => u.Id == id);
        }
        else {
          // This should really be HttpUnauthorizedResult, but that results
          // in an infinite login/redirect loop. Should fix later
          return new HttpNotFoundResult();
        }
      }
      else {
        user = repository.All<User>().Single(u => u.Username.Equals(principal.Identity.Name));
      }
      var currentSeason = repository.All<Season>().Single(s => s.IsActive);
      var team = repository.All<Team>().SingleOrDefault(t => t.Division.Season.Id == currentSeason.Id && t.Players.Select(p => p.Id).Contains(user.Id));
      var viewModel = CreateIndexViewModel(user, team, repository);

      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View(viewModel);
    }

    private DashboardViewModel CreateIndexViewModel(User user, Team team, IRepository repository) {
      var model = new DashboardViewModel();
      model.UserIsAdmin = user.IsInRole(Roles.Administrators);
      model.UserFullName = user.FullName;
      model.SkillLevelCalculation = new SkillLevelCalculationViewModel(user, repository);
      if (null != team) {
        model.CurrentSeasonStats = GetCurrentSeasonStatsViewModel(user, team);
        model.HasCurrentSeasonStats = model.CurrentSeasonStats != null;
        model.LastMeetStats = GetLastMeetStats(user, team);
        model.HasLastMeetStats = model.LastMeetStats != null;
        model.SeasonResults = GetSeasonResults(user, team);
        model.HasSeasonResults = model.SeasonResults != null;
      }
      return model;
    }

    private IEnumerable<SeasonResultViewModel> GetSeasonResults(User user, Team team) {
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

    private LastMeetViewModel GetLastMeetStats(User user, Team team) {
      LastMeetViewModel viewModel = null;
      var meet = (from m in team.Division.Meets
                  where m.Teams.Contains(team) && m.IsComplete
                  orderby m.Week descending
                  select m).FirstOrDefault();
      if (null != meet) {
        viewModel = CreateLastMeetViewModel(meet, team);
      }
      return viewModel;
    }

    private StatsViewModel GetCurrentSeasonStatsViewModel(User user, Team team) {
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
      return (wins + losses > 0) ? ((double)wins / (double)(wins + losses)) : 0;
    }

    private LastMeetViewModel CreateLastMeetViewModel(Meet meet, Team team) {
      var model = new LastMeetViewModel();
      model.OpponentTeam = meet.Teams.Where(t => t != team).Single().Name;
      var matches = new List<LastMatchViewModel>();
      foreach (var match in meet.Matches) {
        var matchvm = CreateLastMatchViewModel(match);
        matches.Add(matchvm);
      }
      model.Matches = matches;
      return model;
    }

    private LastMatchViewModel CreateLastMatchViewModel(Match match) {
      var model = new LastMatchViewModel();
      if (!match.IsForfeit) {
        var results = new List<MatchResultViewModel>();
        foreach (var result in match.Results) {
          var resultvm = new MatchResultViewModel(result);
          resultvm.Winner = match.Winner == result.Player;
          results.Add(resultvm);
        }
        model.Results = results;
      }
      else {
        // this match was a forfeit, create results to display
        var results = new List<MatchResultViewModel>();
        foreach (var matchPlayer in match.Players) {
          var result = new MatchResultViewModel();
          result.Player = matchPlayer.Player.FullName;
          result.Winner = match.Winner == matchPlayer.Player;
          results.Add(result);
        }
        model.Results = results;
      }
      return model;
    }

    private SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        var alertsGadget = new AlertsSidebarGadget();
        sidebarGadgetCollection.Add(AlertsSidebarGadget.Name, alertsGadget);
      }
      return sidebarGadgetCollection;
    }

    private bool userHasAlerts() {
      var hasAlerts = false;
      // unapproved users alert
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        hasAlerts |= repository.All<User>().Any(u => !u.IsApproved);
      }
      return hasAlerts;
    }

    [Authorize]
    public ActionResult AlertsGadget() {
      return PartialView(GetAlertsViewModel());
    }

    private AlertsViewModel GetAlertsViewModel() {
      var warnings = new List<Alert>();
      var errors = new List<Alert>();
      var notifications = new List<Alert>();
      // add unapproved users warning
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        var unapprovedQuery = repository.All<User>().Where(u => !u.IsApproved);
        if (unapprovedQuery.Any()) {
          var url = Url.Action("Unapproved", "Users");
          warnings.Add(new Alert(string.Format("There are {0} users awaiting approval", unapprovedQuery.Count()), url, AlertType.Warning));
        }
      }
      return new AlertsViewModel(notifications, warnings, errors);
    }
  }
}
