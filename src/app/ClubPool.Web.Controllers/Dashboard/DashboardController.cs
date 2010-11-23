using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;

using SharpArch.Core;
using SharpArch.Web.NHibernate;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.Web.Controllers.Dashboard.ViewModels;
using ClubPool.Web.Controllers.Dashboard.SidebarGadgets;
using ClubPool.Framework.NHibernate;

namespace ClubPool.Web.Controllers.Dashboard
{
  public class DashboardController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IUserRepository userRepository;
    protected ISeasonRepository seasonRepository;

    public DashboardController(IAuthenticationService authSvc, IUserRepository userRepository, ISeasonRepository seasonRepository) {
      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != userRepository, "userRepository cannot be null");
      Check.Require(null != seasonRepository, "seasonRepository cannot be null");

      authenticationService = authSvc;
      this.userRepository = userRepository;
      this.seasonRepository = seasonRepository;
    }

    [Authorize]
    [Transaction]
    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      var principal = authenticationService.GetCurrentPrincipal();
      var user = userRepository.FindOne(u => u.Username.Equals(principal.Identity.Name));
      viewModel.UserIsAdmin = principal.IsInRole(Roles.Administrators);
      viewModel.UserFullName = user.FullName;
      viewModel.CurrentSeasonStats = GetCurrentSeasonStatsViewModel(user);
      viewModel.HasCurrentSeasonStats = viewModel.CurrentSeasonStats != null;
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View(viewModel);
    }

    protected StatsViewModel GetCurrentSeasonStatsViewModel(User user) {
      StatsViewModel vm = null;
      // first see if there's a current season
      var currentSeason = seasonRepository.GetAll().Where(s => s.IsActive).FirstOrDefault();
      if (null != currentSeason) {
        // now see if this user is on a team in this season
        var teamQuery = from d in currentSeason.Divisions
                        from t in d.Teams
                        where t.Players.Contains(user)
                        select t;
        var team = teamQuery.FirstOrDefault();
        if (null != team) {
          // if so, compile stats
          vm = new StatsViewModel();
          var skillLevel = user.SkillLevels.Where(sl => sl.GameType == currentSeason.GameType).FirstOrDefault();
          if (null != skillLevel) {
            vm.SkillLevel = skillLevel.Value;
          }
          vm.TeamName = team.Name;
          vm.Teammate = team.Players.Where(p => p != user).Single().FullName;
          var winsAndLosses = team.GetWinsAndLossesForPlayer(user);
          var pct = (double)winsAndLosses[0] / (double)(winsAndLosses[0] + winsAndLosses[1]);
          vm.PersonalRecord = string.Format("{0} - {1} ({2})", winsAndLosses[0], winsAndLosses[1], pct.ToString(".00"));
          winsAndLosses = team.GetWinsAndLosses();
          pct = (double)winsAndLosses[0] / (double)(winsAndLosses[0] + winsAndLosses[1]);
          vm.TeamRecord = string.Format("{0} - {1} ({2})", winsAndLosses[0], winsAndLosses[1], pct.ToString(".00"));
        }
      }
      return vm;
    }

    protected SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        var alertsGadget = new AlertsSidebarGadget();
        sidebarGadgetCollection.Add(AlertsSidebarGadget.Name, alertsGadget);
      }
      return sidebarGadgetCollection;
    }

    protected bool userHasAlerts() {
      var hasAlerts = false;
      // unapproved users alert
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        hasAlerts |= userRepository.GetAll().Where(u => !u.IsApproved).Any();
      }
      return hasAlerts;
    }

    [Authorize]
    [Transaction]
    public ActionResult AlertsGadget() {
      return PartialView(GetAlertsViewModel());
    }

    protected AlertsViewModel GetAlertsViewModel() {
      var warnings = new List<Alert>();
      var errors = new List<Alert>();
      var notifications = new List<Alert>();
      // add unapproved users warning
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        var unapprovedQuery = userRepository.GetAll().Where(u => !u.IsApproved);
        if (unapprovedQuery.Any()) {
          var url = BuildUrlFromExpression<Users.UsersController>(u => u.Unapproved(), null);
          warnings.Add(new Alert(string.Format("There are {0} users awaiting approval", unapprovedQuery.Count()), url, AlertType.Warning));
        }
      }
      if (false) {
        // add dummy errors
        errors.Add(new Alert("Temp dummy error alert", "", AlertType.Error));
        errors.Add(new Alert("Temp dummy error alert with really long message that will wrap", "", AlertType.Error));
        errors.Add(new Alert("Temp dummy error alert", "", AlertType.Error));
        // add dummy warning
        warnings.Add(new Alert("Temp dummy warning alert with really long message that will wrap", "", AlertType.Warning));
        // add dummy notification
        notifications.Add(new Alert("Temp dummy notification alert", "", AlertType.Notification));
        notifications.Add(new Alert("Temp dummy notification alert with really long message that will wrap", "", AlertType.Notification));
        notifications.Add(new Alert("Temp dummy notification alert", "", AlertType.Notification));
      }
      return new AlertsViewModel(notifications, warnings, errors);
    }
  }
}
