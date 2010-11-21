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
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Web.Controls.Captcha;

namespace ClubPool.Web.Controllers.Users
{
  public class UsersController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected IRoleRepository roleRepository;
    protected IEmailService emailService;
    protected IUserRepository userRepository;

    public UsersController(IAuthenticationService authSvc, 
      IMembershipService membershipSvc, 
      IEmailService emailSvc,
      IUserRepository userRepo,
      IRoleRepository roleRepo)
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

    protected void RollbackUserTransaction() {
      userRepository.DbContext.RollbackTransaction();
    }

    [Authorize(Roles=Roles.Administrators)]
    [Transaction]
    public ActionResult Index(int? page) {
      int pageSize = 10;
      var query = from u in userRepository.GetAll()
                  orderby u.LastName, u.FirstName
                  select new UserSummaryViewModel(u);
      var viewModel = new IndexViewModel(query, page.GetValueOrDefault(1), pageSize);
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
        TempData[GlobalViewDataProperty.PageErrorMessage] = "Invalid username/password";
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
    [Transaction]
    public ActionResult ResetPassword(ResetPasswordViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }
      var user = userRepository.FindOne(u => u.Username.Equals(viewModel.Username));
      if (null == user) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "There is no user by that username.";
        return View(viewModel);
      }

      var newPasswords = membershipService.GenerateTempHashedPassword(user.PasswordSalt);
      user.Password = newPasswords[1];
      var body = string.Format("Your ClubPool password has been reset to: {0}", newPasswords[0]);
      emailService.SendSystemEmail(user.Email, "ClubPool password reset", body);
      return View("ResetPasswordComplete");
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
      var user = CreateUser(viewModel, false, false);

      if (null == user) {
        // if we couldn't create the user that means there was some type of validation error,
        // so redisplay the form with the model
        return View(viewModel);
      }

      SendNewUserAwaitingApprovalEmail(user);
      return View("SignUpComplete");
    }

    protected void SendNewUserAwaitingApprovalEmail(User newUser) {
      var officers = roleRepository.FindOne(RoleQueries.RoleByName(Roles.Officers)).Users;
      if (officers.Any()) {
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
      if (null == userToDelete) {
        return HttpNotFound();
      }
      if (userToDelete.CanDelete()) {
        userRepository.Delete(userToDelete);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was deleted successfully.";
      }
      else {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "There is data in the system referencing this user, the user cannot be deleted.";
      }
      return this.RedirectToAction(c => c.Index(page));
    }

    [HttpGet]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Unapproved() {
      var viewModel = new UnapprovedViewModel();
      viewModel.UnapprovedUsers = userRepository.GetAll().WhereUnapproved().ToList()
        .Select(u => new UnapprovedUser(u));
      return View(viewModel);
    }

    [HttpPost]
    [Transaction]
    [ValidateAntiForgeryToken]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Approve(int[] userIds) {
      var users = userRepository.GetAll().WhereIdIn(userIds);
      if (users.Any()) {
        foreach (var user in users) {
          user.IsApproved = true;
        }
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The selected users have been approved.";
      }
      return this.RedirectToAction(c => c.Unapproved());
    }

    [HttpGet]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Edit(int id) {
      var user = userRepository.Get(id);
      var viewModel = new EditViewModel(user);
      viewModel.LoadAvailableRoles(roleRepository);
      return View(viewModel);
    }

    [HttpPost]
    [Transaction]
    [Authorize(Roles=Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }

      var user = userRepository.Get(viewModel.Id);
      if (null == user) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The user you were editing was deleted by another user";
        return this.RedirectToAction(c => c.Index(null));
      }

      if (viewModel.Version != user.Version) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = 
          "This user was updated by another user while you were viewing this page. Enter your changes again.";
        return this.RedirectToAction(c => c.Edit(viewModel.Id));
      }

      if (!user.Username.Equals(viewModel.Username)) {
        // verify that the new username is not in use
        if (membershipService.UsernameIsInUse(viewModel.Username)) {
          ModelState.AddModelErrorFor<EditViewModel>(m => m.Username, "The username is already in use");
          viewModel.LoadAvailableRoles(roleRepository);
          RollbackUserTransaction();
          return View(viewModel);
        }
        user.Username = viewModel.Username;
      }
      if (!user.Email.Equals(viewModel.Email)) {
        // verify that the new email is not in use
        if (membershipService.EmailIsInUse(viewModel.Email)) {
          ModelState.AddModelErrorFor<EditViewModel>(m => m.Email, "The email address is already in use");
          viewModel.LoadAvailableRoles(roleRepository);
          RollbackUserTransaction();
          return View(viewModel);
        }
        user.Email = viewModel.Email;
      }
      user.FirstName = viewModel.FirstName;
      user.LastName = viewModel.LastName;
      user.IsApproved = viewModel.IsApproved;
      user.IsLocked = viewModel.IsLocked;
      user.RemoveAllRoles();
      if (null != viewModel.Roles && viewModel.Roles.Length > 0) {
        foreach (int roleId in viewModel.Roles) {
          user.AddRole(roleRepository.Get(roleId));
        }
      }
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
      var user = CreateUser(viewModel, true, false);

      if (null == user) {
        // if we couldn't create the user that means there was some type of validation error,
        // so redisplay the form with the model
        return View(viewModel);
      }

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was created successfully";
      return this.RedirectToAction(c => c.Index(null));
    }

    protected User CreateUser(CreateViewModel viewModel, bool approved, bool locked) {
      if (!ValidateViewModel(viewModel)) {
        return null;
      }

      if (membershipService.UsernameIsInUse(viewModel.Username)) {
        // the username is in use
        ModelState.AddModelErrorFor<CreateViewModel>(m => m.Username, "The username is already in use");
        return null;
      }

      if (membershipService.EmailIsInUse(viewModel.Email)) {
        // the email address is in use
        ModelState.AddModelErrorFor<CreateViewModel>(m => m.Email, "The email address is already in use");
        return null;
      }
      var user = membershipService.CreateUser(viewModel.Username, viewModel.Password, viewModel.FirstName,
        viewModel.LastName, viewModel.Email, approved, locked);
      return user;
    }

    [HttpGet]
    [Authorize]
    [Transaction]
    public ActionResult View(int id) {
      var user = userRepository.Get(id);
      if (null == user) {
        return HttpNotFound();
      }

      var viewModel = new ViewViewModel(user);
      var principal = authenticationService.GetCurrentPrincipal();
      viewModel.ShowAdminProperties = principal.IsInRole(Roles.Administrators);
      return View(viewModel);
    }
  }
}
