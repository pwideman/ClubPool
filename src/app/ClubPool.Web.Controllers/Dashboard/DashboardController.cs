using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
      if (viewModel.UserIsAdmin) {
        // get the new users awaiting approval, if any
        if (userRepository.GetAll().Where(u => !u.IsApproved).Count() > 0) {
          viewModel.NewUsersAwaitingApproval = userRepository.GetAll().Where(u => !u.IsApproved).AsEnumerable();
        }
      }

      return View(viewModel);
    }
  }
}
