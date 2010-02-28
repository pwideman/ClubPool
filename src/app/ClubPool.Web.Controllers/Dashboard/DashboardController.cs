using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using SharpArch.Core;

using ClubPool.Core;
using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.Web.Controllers.Dashboard.ViewModels;

namespace ClubPool.Web.Controllers
{
  public class DashboardController : BaseController
  {
    protected IRoleService roleService;
    protected IAuthenticationService authenticationService;

    public DashboardController(IRoleService roleSvc, IAuthenticationService authSvc) {
      Check.Require(null != roleSvc, "roleSvc cannot be null");
      Check.Require(null != authSvc, "authSvc cannot be null");

      roleService = roleSvc;
      authenticationService = authSvc;
    }

    [Authorize]
    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      viewModel.UserIsAdmin = roleService.IsUserAdministrator(authenticationService.GetCurrentIdentity().Username);

      return View(viewModel);
    }
  }
}
