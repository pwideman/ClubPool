using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Spark;

using ClubPool.Core;

namespace ClubPool.Web.Controllers
{
  [Precompile]
  public abstract class BaseController : Controller
  {
    protected override void OnResultExecuting(ResultExecutingContext filterContext) {
      var viewResult = filterContext.Result as ViewResultBase;
      if (null != viewResult) {
        var viewModel = viewResult.ViewData.Model as BaseViewModel;
        if (null != viewModel) {
          InitializeCommonPartialViewModels(viewModel);
        }
      }
      base.OnResultExecuting(filterContext);
    }

    //protected LoginStatusViewModel loginStatusViewModel { get; set; }
    //protected MenuViewModel menuViewModel { get; set; }
    //protected BaseViewModel baseViewModel { get; set; }

    //protected override void OnActionExecuting(ActionExecutingContext filterContext) {
    //  base.OnActionExecuting(filterContext);
    //  InitializePartialViewModels();
    //}

    protected virtual void InitializeCommonPartialViewModels(BaseViewModel viewModel) {
      viewModel.LoginStatusViewModel = new LoginStatusViewModel() {
        IsLoggedIn = User.Identity.IsAuthenticated,
        Username = User.Identity.Name
      };
      viewModel.MenuViewModel = new MenuViewModel() {
        DisplayAdminMenu = User.IsInRole(Roles.Administrators)
      };
    }

    //protected IMembershipService membershipService;
    //protected IRoleService roleService;

    //public BaseController(IMembershipService membershipSvc, IRoleService roleSvc) {
    //  membershipService = membershipSvc;
    //  roleService = roleSvc;
    //}

    //protected void InitializeViewModel(ViewModelBase viewModel) {
    //  if (User.Identity.IsAuthenticated) {
    //    viewModel.IsLoggedIn = true;
    //    viewModel.Username = User.Identity.Name;
    //    viewModel.Roles = roleService.GetRolesForUser(viewModel.Username);
    //  }
    //  else {
    //    viewModel.IsLoggedIn = false;
    //    viewModel.Username = null;
    //    viewModel.Roles = new string[0];
    //  }
    //}
  }
}
