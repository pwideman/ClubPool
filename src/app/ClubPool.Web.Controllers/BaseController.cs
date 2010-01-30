using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Spark;

using ClubPool.Web.Controllers.ViewModels;
using ClubPool.ApplicationServices.Interfaces;

namespace ClubPool.Web.Controllers
{
  [Precompile]
  public abstract class BaseController : Controller
  {
    protected IMembershipService membershipService;
    protected IRoleService roleService;

    public BaseController(IMembershipService membershipSvc, IRoleService roleSvc) {
      membershipService = membershipSvc;
      roleService = roleSvc;
    }

    protected void InitializeViewModel(ViewModelBase viewModel) {
      if (HttpContext.User.Identity.IsAuthenticated) {
        viewModel.IsLoggedIn = true;
        viewModel.Username = HttpContext.User.Identity.Name;
        viewModel.Roles = roleService.GetRolesForUser(viewModel.Username);
      }
      else {
        viewModel.IsLoggedIn = false;
        viewModel.Username = null;
        viewModel.Roles = new string[0];
      }
    }
  }
}
