using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using ClubPool.Core;
using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.Web.Controllers.Navigation.ViewModels;

namespace ClubPool.Web.Controllers
{
  public class NavigationController : BaseController
  {
    protected IRoleService roleService;
    protected IAuthenticationService authenticationService;

    public NavigationController(IAuthenticationService authSvc, IRoleService roleSvc) {
      roleService = roleSvc;
      authenticationService = authSvc;
    }

    public ActionResult Menu() {
      var viewModel = new MenuViewModel();
      if (authenticationService.IsLoggedIn()) {
        var identity = authenticationService.GetCurrentIdentity();
        viewModel.DisplayAdminMenu = roleService.IsUserInRole(identity.Username, Roles.Administrators);
        viewModel.UserIsLoggedIn = true;
      }
      else {
        viewModel.DisplayAdminMenu = false;
        viewModel.UserIsLoggedIn = false;
      }
      return PartialView(viewModel);
    }

  }
}
