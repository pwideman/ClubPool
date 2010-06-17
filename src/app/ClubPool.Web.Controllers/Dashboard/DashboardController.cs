using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;

using SharpArch.Core;
using SharpArch.Web.NHibernate;

using ClubPool.Core;
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
    protected ILinqRepository<User> userRepository;

    public DashboardController(IAuthenticationService authSvc, ILinqRepository<User> userRepository) {
      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != userRepository, "userRepository cannot be null");

      authenticationService = authSvc;
      this.userRepository = userRepository;
    }

    [Authorize]
    [Transaction]
    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      viewModel.UserIsAdmin = authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators);
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View(viewModel);
    }

    protected SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      var alertsGadget = new AlertsSidebarGadget();
      sidebarGadgetCollection.Add(alertsGadget.Name, alertsGadget);
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
