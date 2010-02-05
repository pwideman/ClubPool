using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Attributes
{
  public class LoginStatusViewDataAttribute : BaseViewDataAttribute
  {
    protected override object GetViewModel(ResultExecutingContext filterContext) {
      var user = filterContext.HttpContext.User;
      var viewModel = new LoginStatusViewModel() {
        UserIsLoggedIn = user.Identity.IsAuthenticated,
        Username = user.Identity.Name
      };
      return viewModel;
    }
  }
}
