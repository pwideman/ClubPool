using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;

using MvcContrib;
using MvcContrib.Attributes;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Messaging.Contracts;
using ClubPool.Web.Controllers.User.ViewModels;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using Core = ClubPool.Core;
using ClubPool.Core.Queries;
using ClubPool.Web.Controls.Captcha;

namespace ClubPool.Web.Controllers
{
  public class UserController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected IRoleService roleService;
    protected ILinqRepository<Core.Player> playerRepository;
    protected IEmailService emailService;

    public UserController(IAuthenticationService authSvc, 
      IMembershipService membershipSvc, 
      IRoleService roleSvc,
      ILinqRepository<Core.Player> playerRepo,
      IEmailService emailSvc) {

      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != membershipSvc, "membershipSvc cannot be null");
      Check.Require(null != roleSvc, "roleSvc cannot be null");
      Check.Require(null != playerRepo, "playerRepo cannot be null");
      Check.Require(null != emailSvc, "emailSvc cannot be null");

      authenticationService = authSvc;
      membershipService = membershipSvc;
      roleService = roleSvc;
      playerRepository = playerRepo;
      emailService = emailSvc;
    }

    public ActionResult Index() {
      if (authenticationService.IsLoggedIn()) {
        return this.RedirectToAction<HomeController>(x => x.Index());
      }

      return this.RedirectToAction(x => x.Login(new LoginViewModel()));
    }

    [AcceptGet]
    public ActionResult Login(string returnUrl) {
      if (authenticationService.IsLoggedIn()) {
        return this.RedirectToAction<HomeController>(c => c.Index());
      }
      else {
        return View(new LoginViewModel() { ReturnUrl = returnUrl });
      }
    }

    [AcceptPost]
    public ActionResult Login(LoginViewModel viewModel) {
      if (membershipService.ValidateUser(viewModel.Username, viewModel.Password)) {
        authenticationService.LogIn(viewModel.Username, viewModel.StayLoggedIn);
        if (!string.IsNullOrEmpty(viewModel.ReturnUrl)) {
          return this.Redirect(viewModel.ReturnUrl);
        }
        else {
          return this.RedirectToAction<DashboardController>(x => x.Index());
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
        UserIsLoggedIn = authenticationService.IsLoggedIn(),
      };
      if (viewModel.UserIsLoggedIn) {
        viewModel.Username = authenticationService.GetCurrentIdentity().Username;
      }
      return PartialView(viewModel);
    }

    public ActionResult LoginGadget() {
      return PartialView(new LoginViewModel());
    }

    public ActionResult AccountHelp() {
      return View();
    }

    [AcceptGet]
    public ActionResult ResetPassword() {
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

    [AcceptGet]
    public ActionResult SignUp() {
      return View(new SignUpViewModel());
    }

    [AcceptPost]
    [ValidateAntiForgeryToken]
    [Transaction]
    [CaptchaValidation("captcha")]
    public ActionResult SignUp(SignUpViewModel viewModel, bool captchaValid) {
      if (!captchaValid) {
        ModelState.AddModelError("captcha", "Incorrect. Try again.");
      }
      else {
        try {
          viewModel.Validate();
          membershipService.CreateUser(viewModel.Username, viewModel.Password, viewModel.Email, false);
          SendNewPlayerAwaitingApprovalEmail(viewModel.Username, viewModel.FirstName, viewModel.LastName, viewModel.Email);
          return View("SignUpComplete");
        }
        catch (RulesException re) {
          re.AddModelStateErrors(this.ModelState, null);
        }
        catch (MembershipCreateUserException me) {
          viewModel.ErrorMessage = me.Message;
        }
      }
      return View(viewModel);
    }

    protected void SendNewPlayerAwaitingApprovalEmail(string username, string firstName, string lastName, string email) {
      var adminUsernames = roleService.GetUsersInRole(Core.Roles.Administrators);
      var adminPlayers = playerRepository.GetAll().WithUsernames(adminUsernames);
      var adminEmailAddresses = adminPlayers.Select(p => p.User.Email).ToList();
      if (adminEmailAddresses.Count > 0) {
        var subject = "New player sign up at ClubPool";
        var body = new StringBuilder();
        body.Append("A new player has signed up at ClubPool and needs admin approval:" + Environment.NewLine);
        body.Append(string.Format("Username: {0}" + Environment.NewLine + "Name: {1} {2}" + Environment.NewLine + "Email: {3}",
          username, firstName, lastName, email));
        emailService.SendSystemEmail(adminEmailAddresses, subject, body.ToString());
      }
    }

    [Authorize(Roles=Core.Roles.Administrators)]
    public ActionResult Delete(int id) {
      return View();
    }
  }
}
