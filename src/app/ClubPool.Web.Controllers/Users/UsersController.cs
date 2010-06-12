using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Messaging.Contracts;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Queries;
using ClubPool.Web.Controls.Captcha;
using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers.Users
{
  public class UsersController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected ILinqRepository<Role> roleRepository;
    protected IEmailService emailService;
    protected ILinqRepository<User> userRepository;

    public UsersController(IAuthenticationService authSvc, 
      IMembershipService membershipSvc, 
      IEmailService emailSvc,
      ILinqRepository<User> userRepo,
      ILinqRepository<Role> roleRepo)
    {

      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != membershipSvc, "membershipSvc cannot be null");
      Check.Require(null != userRepo, "userRepo cannot be null");
      Check.Require(null != roleRepo, "roleRepo cannot be null");
      Check.Require(null != emailSvc, "emailSvc cannot be null");


      authenticationService = authSvc;
      membershipService = membershipSvc;
      emailService = emailSvc;
      userRepository = userRepo;
      roleRepository = roleRepo;
    }

    [Authorize(Roles=Roles.Administrators)]
    [Transaction]
    public ActionResult Index(int? page) {
      int pageSize = 10;
      var index = Math.Max(page ?? 1, 1) - 1;
      page = page.GetValueOrDefault(1);
      var users = userRepository.GetAll().Select(u => new UserDto(u)).AsPagination(page.Value, pageSize);
      var viewModel = new IndexViewModel() {
        Users = users,
        Page = page.Value
      };
      return View(viewModel);
    }

    [HttpGet]
    public ActionResult Login(string returnUrl) {
      if (authenticationService.IsLoggedIn()) {
        if (null != returnUrl && !string.IsNullOrEmpty(returnUrl)) {
          return Redirect(returnUrl);
        }
        else {
          return this.RedirectToAction<Dashboard.DashboardController>(c => c.Index());
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
          return this.RedirectToAction<Dashboard.DashboardController>(x => x.Index());
        }
      }
      else {
        viewModel.Password = "";
        viewModel.ErrorMessage = "Invalid username/password";
        return View(viewModel);
      }
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

    public ActionResult LoginGadget() {
      return PartialView(new LoginViewModel());
    }

    public ActionResult AccountHelp() {
      return View();
    }

    [HttpGet]
    public ActionResult ResetPassword() {
      return View();
    }

    [HttpPost]
    public ActionResult ResetPassword(ResetPasswordViewModel viewModel) {
      return View();
    }

    public ActionResult Logout() {
      authenticationService.LogOut();
      return this.RedirectToAction<Home.HomeController>(x => x.Index());
    }

    [HttpGet]
    public ActionResult SignUp() {
      return View(new SignUpViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Transaction]
    [CaptchaValidation("captcha")]
    public ActionResult SignUp(SignUpViewModel viewModel, bool captchaValid) {
      if (!captchaValid) {
        ModelState.AddModelError("captcha", "Incorrect. Try again.");
        return View(viewModel);
      }
      var user = CreateUser(viewModel, false);

      if (null == user) {
        // if we couldn't create the user that means there was some type of validation error,
        // so redisplay the form with the model
        return View(viewModel);
      }

      SendNewUserAwaitingApprovalEmail(user);
      return View("SignUpComplete");
    }

    protected void SendNewUserAwaitingApprovalEmail(User newUser) {
      //var adminUsernames = roleService.GetUsersInRole(Core.Roles.Administrators);
      var officers = roleRepository.FindOne(RoleQueries.RoleByName(Core.Roles.Officers)).Users;
      if (officers.Count() > 0) {
        var officerEmailAddresses = officers.Select(u => u.Email).ToList();
        var subject = "New user sign up at ClubPool";
        var body = new StringBuilder();
        body.Append("A new user has signed up at ClubPool and needs admin approval:" + Environment.NewLine);
        body.Append(string.Format("Username: {0}" + Environment.NewLine + "Name: {1} {2}" + Environment.NewLine + "Email: {3}",
          newUser.Username, newUser.FirstName, newUser.LastName, newUser.Email));
        emailService.SendSystemEmail(officerEmailAddresses, subject, body.ToString());
      }
    }

    [Authorize(Roles=Roles.Administrators)]
    [HttpPost]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, int page) {
      User userToDelete = userRepository.Get(id);
      if (null != userToDelete) {
        //TODO: check to see if user has any data and if so, deny delete
        userRepository.Delete(userToDelete);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was deleted successfully.";
      }
      else {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "Invalid user id";
      }
      return this.RedirectToAction(c => c.Index(page));
    }

    [HttpGet]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Unapproved() {
      var viewModel = new UnapprovedViewModel();
      viewModel.UnapprovedUsers = userRepository.GetAll().WhereUnapproved().ToList()
        .Select(u => new UnapprovedUser() { FullName = u.FullName, Email = u.Email, Id = u.Id });
      return View(viewModel);
    }

    [HttpPost]
    [Transaction]
    [ValidateAntiForgeryToken]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Approve(int[] userIds) {
      var users = userRepository.GetAll().WhereIdIn(userIds);
      if (users.Count() > 0) {
        foreach (var user in users) {
          user.IsApproved = true;
        }
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The selected users have been approved.";
      }
      return this.RedirectToAction(c => c.Unapproved());
    }

    [Transaction]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult View(int id) {
      var user = new UserDto(userRepository.Get(id));
      return View(user);
    }

    [HttpGet]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Edit(int id) {
      var user = userRepository.Get(id);
      var viewModel = new EditViewModel() {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        IsApproved = user.IsApproved
      };
      return View(viewModel);
    }

    [HttpPost]
    [Transaction]
    [Authorize(Roles=Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditViewModel viewModel) {
      try {
        viewModel.Validate();
      }
      catch (RulesException re) {
        re.AddModelStateErrors(this.ModelState, null);
        return View(viewModel);
      }

      var user = userRepository.Get(viewModel.Id);
      user.Username = viewModel.Username;
      user.FirstName = viewModel.FirstName;
      user.LastName = viewModel.LastName;
      user.Email = viewModel.Email;
      user.IsApproved = viewModel.IsApproved;
      userRepository.SaveOrUpdate(user);

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was updated successfully";
      return this.RedirectToAction(c => c.Index(null));
    }

    [HttpGet]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Create() {
      var viewModel = new CreateViewModel();
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles=Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateViewModel viewModel) {
      var user = CreateUser(viewModel, true);

      if (null == user) {
        // if we couldn't create the user that means there was some type of validation error,
        // so redisplay the form with the model
        return View(viewModel);
      }

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was created successfully";
      return this.RedirectToAction(c => c.Index(null));
    }

    protected User CreateUser(CreateViewModel viewModel, bool approved) {
      try {
        viewModel.Validate();
      }
      catch (RulesException re) {
        re.AddModelStateErrors(this.ModelState, null);
        return null;
      }

      if (membershipService.UsernameIsInUse(viewModel.Username)) {
        // the username is in use
        viewModel.ErrorMessage = string.Format("The username '{0}' is already in use", viewModel.Username);
        return null;
      }

      if (membershipService.EmailIsInUse(viewModel.Email)) {
        // the email address is in use
        viewModel.ErrorMessage = string.Format("The email address '{0}' is already in use", viewModel.Email);
        return null;
      }
      var user = membershipService.CreateUser(viewModel.Username, viewModel.Password, viewModel.FirstName, viewModel.LastName, viewModel.Email, approved);
      return user;
    }
  }
}
