using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;

using SharpArch.Core;

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
    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      viewModel.UserIsAdmin = authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators);
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View(viewModel);
    }

    protected SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      if (userHasAlerts()) {
        var alertsGadget = new AlertsSidebarGadget();
        sidebarGadgetCollection.Add(alertsGadget.Name, alertsGadget);
      }
      return sidebarGadgetCollection;
    }

    protected bool userHasAlerts() {
      var hasAlerts = false;
      // unapproved users alert
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        var numberOfUnapprovedUsers = userRepository.GetAll().Where(u => !u.IsApproved).Count();
        hasAlerts |= numberOfUnapprovedUsers > 0;
      }
      return hasAlerts;
    }

    [Authorize]
    public ActionResult AlertsGadget() {
      var alerts = new List<Alert>();
      var viewModel = new AlertsGadgetViewModel(alerts);
      if (authenticationService.GetCurrentPrincipal().IsInRole(Roles.Administrators)) {
        var numberOfUnapprovedUsers = userRepository.GetAll().Where(u => !u.IsApproved).Count();
        if (numberOfUnapprovedUsers > 0) {
          var url = BuildUrlFromExpression<Users.UsersController>(u => u.Unapproved(), null);
          alerts.Add(new Alert(string.Format("There are {0} users awaiting approval", numberOfUnapprovedUsers), url));
        }
      }
      return PartialView(viewModel);
    }
  }
}
