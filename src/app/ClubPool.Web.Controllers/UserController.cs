using System;
using System.Web.Mvc;

using MvcContrib;

using ClubPool.ApplicationServices.Interfaces;

namespace ClubPool.Web.Controllers
{
  public class UserController : BaseController
  {
    private IAuthenticationService authenticationService;
    private IMembershipService membershipService;

    public UserController(IAuthenticationService authSvc, IMembershipService memberSvc) {
      authenticationService = authSvc;
      membershipService = memberSvc;
    }

    public ActionResult Index() {
      if (authenticationService.IsLoggedIn()) {
        return this.RedirectToAction<HomeController>(x => x.Index());
      }

      return this.RedirectToAction(x => x.Login(null));
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Login(string returnUrl) {
      return View(new UserLoginViewModel() { ReturnUrl = returnUrl });
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Login(string username, string password, string returnUrl) {
      if (membershipService.ValidateUser(username, password)) {
        authenticationService.LogIn(username, false);
        if (!string.IsNullOrEmpty(returnUrl)) {
          return this.Redirect(returnUrl);
        }
        else {
          return this.RedirectToAction<HomeController>(x => x.Index());
        }
      }
      else {
        return View(new UserLoginViewModel() { Message = "Invalid username/password", ReturnUrl = returnUrl });
      }
    }
  }

  public class UserLoginViewModel : BaseViewModel
  {
    public string Message { get; set; }
    public string ReturnUrl { get; set; }
  }
}
