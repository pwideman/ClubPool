using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Services.Membership;

namespace ClubPool.Web.Controllers.Login
{
  public class LoginController : Controller, IRouteRegistrar
  {
    private IAuthenticationService authenticationService;
    private IMembershipService membershipService;
    private IRepository repository;

    public LoginController(IAuthenticationService authSvc, 
      IMembershipService membershipSvc, 
      IRepository repository)
    {

      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(membershipSvc, "membershipSvc");
      Arg.NotNull(repository, "repository");

      authenticationService = authSvc;
      membershipService = membershipSvc;
      this.repository = repository;
    }

    [HttpGet]
    public ActionResult Login(string returnUrl) {
      if (authenticationService.IsLoggedIn()) {
        if (null != returnUrl && !string.IsNullOrEmpty(returnUrl)) {
          return Redirect(returnUrl);
        }
        else {
          return RedirectToAction("Index", "Dashboard");
        }
      }
      else {
        return View(new LoginViewModel() { ReturnUrl = returnUrl });
      }
    }

    [HttpPost]
    public ActionResult Login(LoginViewModel viewModel) {
      if (membershipService.ValidateUser(viewModel.Username, viewModel.Password)) {
        authenticationService.LogIn(viewModel.Username, viewModel.StayLoggedIn);
        if (!string.IsNullOrEmpty(viewModel.ReturnUrl)) {
          return this.Redirect(viewModel.ReturnUrl);
        }
        else {
          return RedirectToAction("Index", "Dashboard");
        }
      }
      else {
        viewModel.Password = "";
        TempData[GlobalViewDataProperty.PageErrorMessage] = "Invalid username/password";
        return View(viewModel);
      }
    }

    [HttpGet]
    public ActionResult Logout() {
      authenticationService.LogOut();
      return RedirectToAction("Index", "Home");
    }

    public ActionResult LoginGadget() {
      return PartialView(new LoginViewModel());
    }

    public ActionResult LoginStatus() {
      var principal = authenticationService.GetCurrentPrincipal();
      var viewModel = new LoginStatusViewModel() {
        UserIsLoggedIn = principal.Identity.IsAuthenticated
      };
      if (viewModel.UserIsLoggedIn) {
        viewModel.Username = principal.Identity.Name;
      }
      return PartialView(viewModel);
    }

    public void RegisterRoutes(System.Web.Routing.RouteCollection routes) {
      routes.MapRoute("Login", "login", new { controller = "Login", action = "login" });
      routes.MapRoute("Logout", "logout", new { controller = "Login", action = "logout" });
    }

  }
}
