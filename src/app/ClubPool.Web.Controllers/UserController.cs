using System;
using System.Web.Mvc;

using MvcContrib;
using MvcContrib.Attributes;


using ClubPool.ApplicationServices.Interfaces;
using ClubPool.Web.Controllers.ViewModels;

namespace ClubPool.Web.Controllers
{
  public class UserController : BaseController
  {
    private IAuthenticationService authenticationService;

    public UserController(IAuthenticationService authSvc, IMembershipService membershipSvc, IRoleService roleSvc)
      : base(membershipSvc, roleSvc) {
      authenticationService = authSvc;
    }

    public ActionResult Index() {
      if (authenticationService.IsLoggedIn()) {
        return this.RedirectToAction<HomeController>(x => x.Index());
      }

      return this.RedirectToAction(x => x.Login(new UserLoginViewModel()));
    }

    [AcceptGet]
    public ActionResult Login(string returnUrl) {
      return View(new UserLoginViewModel() { ReturnUrl = returnUrl });
    }

    [AcceptPost]
    public ActionResult Login(UserLoginViewModel viewModel) {
      if (membershipService.ValidateUser(viewModel.Username, viewModel.Password)) {
        authenticationService.LogIn(viewModel.Username, viewModel.RememberMe);
        if (!string.IsNullOrEmpty(viewModel.ReturnUrl)) {
          return this.Redirect(viewModel.ReturnUrl);
        }
        else {
          return this.RedirectToAction<HomeController>(x => x.Index());
        }
      }
      else {
        viewModel.Password = "";
        viewModel.Message = "Invalid username/password";
        return View(viewModel);
      }
    }

    public ActionResult Logout() {
      authenticationService.LogOut();
      return this.RedirectToAction<HomeController>(x => x.Index());
    }
  }
}
