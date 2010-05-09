using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Attributes;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;
using AutoMapper;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Messaging.Contracts;
using ClubPool.Web.Controllers.User.ViewModels;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Queries;
using ClubPool.Web.Controls.Captcha;
using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers
{
  public class UserController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected IRoleService roleService;
    protected IEmailService emailService;
    protected ILinqRepository<Core.User> userRepository;

    public UserController(IAuthenticationService authSvc, 
      IMembershipService membershipSvc, 
      IRoleService roleSvc,
      IEmailService emailSvc,
      ILinqRepository<Core.User> userRepo)
    {

      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != membershipSvc, "membershipSvc cannot be null");
      Check.Require(null != roleSvc, "roleSvc cannot be null");
      Check.Require(null != userRepo, "userRepo cannot be null");
      Check.Require(null != emailSvc, "emailSvc cannot be null");

      authenticationService = authSvc;
      membershipService = membershipSvc;
      roleService = roleSvc;
      emailService = emailSvc;
      userRepository = userRepo;
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
        viewModel.ErrorMessage = "Invalid username/password";
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
        return View(viewModel);
      }
      try {
        viewModel.Validate();
      }
      catch (RulesException re) {
        re.AddModelStateErrors(this.ModelState, null);
        return View(viewModel);
      }

      if (membershipService.UsernameIsInUse(viewModel.Username)) {
        // the username is in use
        viewModel.ErrorMessage = string.Format("The username '{0}' is already in use", viewModel.Username);
        return View(viewModel);
      }

      if (membershipService.EmailIsInUse(viewModel.Email)) {
        // the email address is in use
        viewModel.ErrorMessage = string.Format("The email address '{0}' is already in use", viewModel.Email);
        return View(viewModel);
      }
      var user = membershipService.CreateUser(viewModel.Username, viewModel.Password, viewModel.FirstName, viewModel.LastName, viewModel.Email, false);
      SendNewUserAwaitingApprovalEmail(user);
      return View("SignUpComplete");
    }

    protected void SendNewUserAwaitingApprovalEmail(Core.User newUser) {
      var adminUsernames = roleService.GetUsersInRole(Core.Roles.Administrators);
      if (adminUsernames.Length > 0) {
        var adminUsers = userRepository.GetAll().WithUsernames(adminUsernames);
        var adminEmailAddresses = adminUsers.Select(u => u.Email).ToList();
        var subject = "New user sign up at ClubPool";
        var body = new StringBuilder();
        body.Append("A new user has signed up at ClubPool and needs admin approval:" + Environment.NewLine);
        body.Append(string.Format("Username: {0}" + Environment.NewLine + "Name: {1} {2}" + Environment.NewLine + "Email: {3}",
          newUser.Username, newUser.FirstName, newUser.LastName, newUser.Email));
        emailService.SendSystemEmail(adminEmailAddresses, subject, body.ToString());
      }
    }

    [Authorize(Roles=Core.Roles.Administrators)]
    public ActionResult Delete(int id) {
      return View();
    }

    [Authorize(Roles=Core.Roles.Administrators)]
    public ActionResult Unapproved() {
      var viewModel = new UnapprovedViewModel();
      Mapper.CreateMap<Core.User, UnapprovedUser>();
      viewModel.UnapprovedUsers = 
        Mapper.Map<IList<Core.User>, IEnumerable<UnapprovedUser>>(userRepository.GetAll().WhereUnapproved().ToList());
      return View(viewModel);
    }

    [AcceptPost]
    [Transaction]
    [ValidateAntiForgeryToken]
    [Authorize(Roles=Core.Roles.Administrators)]
    public ActionResult Approve(int[] userIds) {
      var users = userRepository.GetAll().WhereIdIn(userIds);
      if (users.Count() > 0) {
        foreach (var user in users) {
          user.IsApproved = true;
        }
        TempData["message"] = "The selected users have been approved.";
      }
      return MvcContrib.ControllerExtensions.RedirectToAction(this, c => c.Unapproved());
    }
  }
}
