using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using ClubPool.Core;
using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.Web.Controllers.Navigation.ViewModels;

namespace ClubPool.Web.Controllers.Navigation
{
  public class NavigationController : BaseController
  {
    protected IAuthenticationService authenticationService;

    public NavigationController(IAuthenticationService authSvc) {
      authenticationService = authSvc;
    }

    public ActionResult Menu() {
      var viewModel = new MenuViewModel();
      if (authenticationService.IsLoggedIn()) {
        var principal = authenticationService.GetCurrentPrincipal();
        viewModel.DisplayAdminMenu = principal.IsInRole(Roles.Administrators);
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
