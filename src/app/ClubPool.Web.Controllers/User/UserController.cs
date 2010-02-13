using System;
using System.Web.Mvc;

using MvcContrib;
using MvcContrib.Attributes;


using ClubPool.ApplicationServices.Contracts;
using ClubPool.Web.Controllers.User.ViewModels;

namespace ClubPool.Web.Controllers
{
  public class UserController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected IRoleService roleService;

    public UserController(IAuthenticationService authSvc, IMembershipService membershipSvc, IRoleService roleSvc) {
      authenticationService = authSvc;
      membershipService = membershipSvc;
      roleService = roleSvc;
    }

    public ActionResult Index() {
      if (authenticationService.IsLoggedIn()) {
        return this.RedirectToAction<HomeController>(x => x.Index());
      }

      return this.RedirectToAction(x => x.Login(new LoginViewModel()));
    }

    [AcceptGet]
    public ActionResult Login(string returnUrl) {
      return View(new LoginViewModel() { ReturnUrl = returnUrl });
    }

    [AcceptPost]
    public ActionResult Login(LoginViewModel viewModel) {
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

    public ActionResult LoginStatus() {
      var viewModel = new LoginStatusViewModel() {
        UserIsLoggedIn = User.Identity.IsAuthenticated,
        Username = User.Identity.Name
      };
      return PartialView(viewModel);
    }

    public ActionResult LoginGadget() {
      return PartialView(new LoginViewModel() { IsInSidebar = true });
    }

    [AcceptGet]
    public ViewResult ResetPassword() {
      return View();
    }

    [AcceptPost]
    public ActionResult ResetPassword(ResetPasswordViewModel viewModel) {
      return View();
    }

    public ActionResult Logout() {
      authenticationService.LogOut();
      return this.RedirectToAction<HomeController>(x => x.Index());
    }
  }
}
