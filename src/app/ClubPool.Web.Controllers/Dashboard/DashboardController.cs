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
using ClubPool.Framework.NHibernate;

namespace ClubPool.Web.Controllers
{
  public class DashboardController : BaseController
  {
    protected IRoleService roleService;
    protected IAuthenticationService authenticationService;
    protected ILinqRepository<Core.User> userRepository;

    public DashboardController(IRoleService roleSvc, IAuthenticationService authSvc, ILinqRepository<Core.User> userRepository) {
      Check.Require(null != roleSvc, "roleSvc cannot be null");
      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != userRepository, "userRepository cannot be null");

      roleService = roleSvc;
      authenticationService = authSvc;
      this.userRepository = userRepository;
    }

    [Authorize]
    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      viewModel.UserIsAdmin = roleService.IsUserAdministrator(authenticationService.GetCurrentIdentity().Username);
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex(viewModel.UserIsAdmin);
      ViewData[sidebarGadgetCollection.GetType().FullName] = sidebarGadgetCollection;
      return View(viewModel);
    }

    protected SidebarGadgetCollection GetSidebarGadgetCollectionForIndex(bool isAdmin) {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      var alertsGadget = new Dashboard.SidebarGadgets.AlertsSidebarGadget();
      sidebarGadgetCollection.Add(alertsGadget.Name, alertsGadget);
      return sidebarGadgetCollection;
    }

    public ActionResult AlertsGadget() {
      var alerts = new List<Alert>();
      var viewModel = new AlertsGadgetViewModel(alerts);
      if (roleService.IsUserAdministrator(authenticationService.GetCurrentIdentity().Username)) {
        var numberOfUnapprovedUsers = userRepository.GetAll().Where(u => !u.IsApproved).Count();
        if (numberOfUnapprovedUsers > 0) {
          alerts.Add(new Alert(string.Format("There are {0} users awaiting approval", numberOfUnapprovedUsers),
            VirtualPathUtility.ToAbsolute("~/user/unapproved")));
        }
      }
      return PartialView(viewModel);
    }
  }
}
