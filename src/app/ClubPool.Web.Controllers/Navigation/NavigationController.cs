using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using ClubPool.Core;
using ClubPool.ApplicationServices.Interfaces;
//using ClubPool.Web.Controllers.Navigation.ViewModels;

namespace ClubPool.Web.Controllers.Navigation
{
  //public class NavigationController : BaseController
  //{
  //  protected IRoleService roleService;

  //  public NavigationController(IRoleService roleSvc) {
  //    roleService = roleSvc;
  //  }

  //  public ActionResult Menu() {
  //    var viewModel = new MenuViewModel();
  //    if (User.Identity.IsAuthenticated) {
  //      viewModel.DisplayAdminMenu = roleService.IsUserInRole(User.Identity.Name, Roles.Administrators);
  //    }
  //    else {
  //      viewModel.DisplayAdminMenu = false;
  //    }
  //    return PartialView(viewModel);
  //  }

  //}
}
