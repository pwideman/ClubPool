using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using ClubPool.Core;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Attributes
{
  public class MenuViewDataAttribute : BaseViewDataAttribute
  {
    protected override object GetViewModel(ResultExecutingContext filterContext) {
      var user = filterContext.HttpContext.User;
      var viewModel = new MenuViewModel() { UserIsLoggedIn = user.Identity.IsAuthenticated };
      if (viewModel.UserIsLoggedIn) {
        viewModel.DisplayAdminMenu = user.IsInRole(Roles.Administrators);
      }
      else {
        viewModel.DisplayAdminMenu = false;
      }
      return viewModel;
    }
  }
}
