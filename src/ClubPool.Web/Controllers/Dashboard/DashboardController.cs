using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Dashboard.ViewModels;
using ClubPool.Web.Controllers.Dashboard.SidebarGadgets;

namespace ClubPool.Web.Controllers.Dashboard
{
  public class DashboardController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IRepository repository;

    public DashboardController(IAuthenticationService authSvc,
      IRepository repository) {

      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(repository, "repository");

      authenticationService = authSvc;
      this.repository = repository;
    }

    [Authorize]
    // needs transaction
    public ActionResult Index(int? id) {
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
      var viewModel = new IndexViewModel(user, team, repository);

      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View(viewModel);
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
        hasAlerts |= repository.All<User>().Any(u => !u.IsApproved);
      }
      return hasAlerts;
    }

    [Authorize]
    // needs transaction
    public ActionResult AlertsGadget() {
      return PartialView(GetAlertsViewModel());
    }

    protected AlertsViewModel GetAlertsViewModel() {
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
