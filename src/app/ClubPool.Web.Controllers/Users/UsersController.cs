using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;
using System.Web.Routing;
using System.Web;

using Microsoft.Web.Mvc;
//using MvcContrib;
using MvcContrib.ActionResults;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Authentication;
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
      if (string.IsNullOrEmpty(viewModel.Username) && string.IsNullOrEmpty(viewModel.Email)) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "You must enter a username or email address";
        return View();
      }

      string token = "";
      User user = null;

      if (!string.IsNullOrEmpty(viewModel.Username)) {
        user = userRepository.FindOne(u => u.Username.Equals(viewModel.Username));
      }
      else if (!string.IsNullOrEmpty(viewModel.Email)) {
        user = userRepository.GetAll().Where(u => u.Email.Equals(viewModel.Username)).FirstOrDefault();
      }

      if (null != user) {
        // we found a user, send the email containing reset token
        token = membershipService.GeneratePasswordResetToken(user);
        var helper = new UrlHelper(((MvcHandler)HttpContext.CurrentHandler).RequestContext);
        var url = helper.Action("ValidatePasswordResetToken", "Users", new { token = token }, HttpContext.Request.Url.Scheme);
        var body = string.Format("You have requested to reset your password at ClubPool. Click the following link to be logged into your" +
          " account and taken to the member info page, where you can change your password. The link is valid for 24 hours." + Environment.NewLine + Environment.NewLine +
          "WARNING: IF YOU DID NOT REQUEST THIS EMAIL, DO NOT CLICK THE LINK BELOW, AND ALERT THE SITE ADMINISTRATOR" + Environment.NewLine + Environment.NewLine +
          url);
        emailService.SendSystemEmail(user.Email, "ClubPool password reset", body);
      }
      // always go to the complete page, we don't want potential attackers to be able to
      // discover valid usernames through this interface
      return View("ResetPasswordComplete");
    }

    [HttpGet]
    [Transaction]
    public ActionResult ValidatePasswordResetToken(string token) {
      var users = userRepository.GetAll();
      foreach (var user in users) {
        if (membershipService.ValidatePasswordResetToken(token, user)) {
          authenticationService.LogIn(user.Username, false);
          return this.RedirectToAction(c => c.Edit(user.Id));
        }
      }
      // if we got here, the token is not valid
      TempData[GlobalViewDataProperty.PageErrorMessage] = "The reset password link that you clicked on is invalid, enter your information again";
      return this.RedirectToAction(c => c.ResetPassword());
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
    [Authorize]
    public ActionResult Edit(int id) {
      var user = userRepository.Get(id);
      var currentPrincipal = authenticationService.GetCurrentPrincipal();
      var canEditUser = CanEditUser(currentPrincipal, user);
      if (!canEditUser) {
        return ErrorView("You are not authorized to edit this user");
      }
      var canEditStatus = CanEditUserStatus(currentPrincipal, user);
      var canEditRoles = CanEditUserRoles(currentPrincipal, user);
      var viewModel = new EditViewModel(user);
      viewModel.ShowStatus = canEditStatus;
      viewModel.ShowRoles = canEditRoles;
      if (canEditRoles) {
        viewModel.LoadAvailableRoles(roleRepository);
      }
      return View(viewModel);
    }


    // TODO: These 3 CanEdit* methods should really be in some type of service,
    // maybe the AuthenticationService?
    protected bool CanEditUser(ClubPoolPrincipal principal, User user) {
      // admins & officers can edit the basic properties of all users,
      // normal users can edit their own basic properties
      var editorIsAdmin = principal.IsInRole(Roles.Administrators);
      var editorIsOfficer = principal.IsInRole(Roles.Officers);
      var editingSelf = user.Id == principal.UserId;
      return editingSelf || editorIsOfficer || editorIsAdmin;
    }

    protected bool CanEditUserStatus(ClubPoolPrincipal principal, User user) {
      // admins can edit the status of all users, officers can edit the status of
      // all other non admins but not themselves, normal users can not edit status
      var editorIsAdmin = principal.IsInRole(Roles.Administrators);
      if (editorIsAdmin) {
        // admins can edit the status of all other users
        return true;
      }
      var editorIsOfficer = principal.IsInRole(Roles.Officers);
      if (!editorIsOfficer) {
        // if the user is neither admin nor officer, can't edit any status
        return false;
      }
      // if we get here, editor is officer, can edit status of other non-admin
      // users but not self
      var editingSelf = user.Id == principal.UserId;
      var userIsAdmin = user.Roles.Where(r => r.Name.Equals(Roles.Administrators)).Any();
      return (!(editingSelf || userIsAdmin));
    }

    protected bool CanEditUserRoles(ClubPoolPrincipal principal, User user) {
      // only admins can edit roles
      return principal.IsInRole(Roles.Administrators);
    }


    [HttpPost]
    [Transaction]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditViewModel viewModel) {
      var user = userRepository.Get(viewModel.Id);
      if (null == user) {
        return ErrorView("The user you were editing was deleted by another user");
      }
      var currentPrincipal = authenticationService.GetCurrentPrincipal();
      var canEditUser = CanEditUser(currentPrincipal, user);
      if (!canEditUser) {
        return ErrorView("You are not authorized to edit this user");
      }
      var canEditStatus = CanEditUserStatus(currentPrincipal, user);
      var canEditRoles = CanEditUserRoles(currentPrincipal, user);
      // must reset these in case we redisplay the form
      viewModel.ShowStatus = canEditStatus;
      viewModel.ShowRoles = canEditRoles;

      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
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
      if (canEditStatus) {
        user.IsApproved = viewModel.IsApproved;
        user.IsLocked = viewModel.IsLocked;
      }
      if (canEditRoles) {
        user.RemoveAllRoles();
        if (null != viewModel.Roles && viewModel.Roles.Length > 0) {
          foreach (int roleId in viewModel.Roles) {
            user.AddRole(roleRepository.Get(roleId));
          }
        }
      }
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was updated successfully";
      return this.RedirectToAction(c => c.Edit(viewModel.Id));
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

    [HttpGet]
    public ActionResult RecoverUsername() {
      return View();
    }

    [HttpPost]
    [Transaction]
    public ActionResult RecoverUsername(RecoverUsernameViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }
      var usernames = userRepository.GetAll().Where(u => u.Email.Equals(viewModel.Email)).Select(u => u.Username).ToList();
      string body = "";
      if (!usernames.Any()) {
        body = string.Format("There are no usernames registered for the email address '{0}'.", viewModel.Email);
      }
      else {
        var bodysb = new StringBuilder(string.Format("The following usernames are registered for the email address '{0}':", viewModel.Email) + Environment.NewLine);
        foreach (var username in usernames) {
          bodysb.Append(Environment.NewLine + username);
        }
        body = bodysb.ToString();
      }
      emailService.SendSystemEmail(viewModel.Email, "ClubPool Username Assistance", body);
      return View("RecoverUsernameComplete");
    }
  }
}
