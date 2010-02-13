using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using ClubPool.Core;
using ClubPool.ApplicationServices.Contracts;
using ClubPool.Web.Controllers.Navigation.ViewModels;

namespace ClubPool.Web.Controllers
{
  public class NavigationController : BaseController
  {
    protected IRoleService roleService;

    public NavigationController(IRoleService roleSvc) {
      roleService = roleSvc;
    }

    public ActionResult Menu() {
      var viewModel = new MenuViewModel();
      if (User.Identity.IsAuthenticated) {
        viewModel.DisplayAdminMenu = roleService.IsUserInRole(User.Identity.Name, Roles.Administrators);
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
