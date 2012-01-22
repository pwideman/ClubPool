using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;
using System.Web;

using Elmah;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controls.Captcha;
using ClubPool.Web.Services.Configuration;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Services.Messaging;
using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Users
{
  public class UsersController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected IRepository repository;
    protected IEmailService emailService;
    protected IConfigurationService configService;

    public UsersController(IAuthenticationService authSvc, 
      IMembershipService membershipSvc, 
      IEmailService emailSvc,
      IConfigurationService configService,
      IRepository repository)
    {

      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(membershipSvc, "membershipSvc");
      Arg.NotNull(emailSvc, "emailSvc");
      Arg.NotNull(configService, "configService");
      Arg.NotNull(repository, "repository");

      authenticationService = authSvc;
      membershipService = membershipSvc;
      emailService = emailSvc;
      this.configService = configService;
      this.repository = repository;
    }

    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Index(int? page, string q) {
      int pageSize = 10;
      IQueryable<User> userQuery;
      if (!string.IsNullOrEmpty(q)) {
        q = HttpUtility.UrlDecode(q);
        var pieces = q.Split(' ').Where(s => !string.IsNullOrEmpty(s));
        var clause = " (username like '%{0}%' or firstname like '%{0}%' or lastname like '%{0}%' or " +
          "'{0}' like '%' + username + '%' or '{0}' like '%' + firstname + '%' or '{0}' like '%' + lastname + '%')";
        var sqlquery = new StringBuilder("select * from clubpool.users where");
        var first = true;
        foreach (var piece in pieces) {
          if (!first) {
            sqlquery.Append(" or");
          }
          else {
            first = false;
          }
          sqlquery.Append(string.Format(clause, piece));
        }
        sqlquery.Append(" order by LastName, FirstName");
        userQuery = repository.SqlQuery<User>(sqlquery.ToString());
      }
      else {
        userQuery = repository.All<User>().OrderBy(u => u.LastName).ThenBy(u => u.FirstName);
      }
      var viewModel = new IndexViewModel(userQuery, page.GetValueOrDefault(1), pageSize, (u) => new UserSummaryViewModel(u));
      if (!string.IsNullOrEmpty(q)) {
        viewModel.SearchQuery = q;
      }
      return View(viewModel);
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
    [ValidateAntiForgeryToken]
    [CaptchaValidation("captcha")]
    public ActionResult ResetPassword(ResetPasswordViewModel viewModel, bool captchaValid) {
      if (string.IsNullOrEmpty(viewModel.Username) && string.IsNullOrEmpty(viewModel.Email)) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "You must enter a username or email address";
        return View();
      }

      if (!captchaValid) {
        ModelState.AddModelError("captcha", "Incorrect. Try again.");
        return View(viewModel);
      }

      string token = "";
      User user = null;

      if (!string.IsNullOrEmpty(viewModel.Username)) {
        user = repository.All<User>().SingleOrDefault(u => u.Username.Equals(viewModel.Username));
      }
      else if (!string.IsNullOrEmpty(viewModel.Email)) {
        user = repository.All<User>().FirstOrDefault(u => u.Email.Equals(viewModel.Email));
      }

      if (null != user) {
        // we found a user, send the email containing reset token
        token = membershipService.GeneratePasswordResetToken(user);
        var helper = new UrlHelper(((MvcHandler)HttpContext.CurrentHandler).RequestContext);
        var siteName = configService.GetConfig().SiteName;
        var url = helper.Action("ValidatePasswordResetToken", "Users", new { token = token }, HttpContext.Request.Url.Scheme);
        var body = string.Format("You have requested to reset your password at {0}. Click the following link to be logged into your" +
          " account and taken to the member info page, where you can change your password. The link is valid for 24 hours.{1}{1}" +
          "WARNING: IF YOU DID NOT REQUEST THIS EMAIL, DO NOT CLICK THE LINK BELOW, AND ALERT THE SITE ADMINISTRATOR{1}{1}{2}",
          siteName, Environment.NewLine, url);
        emailService.SendSystemEmail(user.Email, string.Format("{0} password reset", siteName), body);
      }
      // always go to the complete page, we don't want potential attackers to be able to
      // discover valid usernames through this interface
      return View("ResetPasswordComplete");
    }

    [HttpGet]
    public ActionResult ValidatePasswordResetToken(string token) {
      var users = repository.All<User>();
      foreach (var user in users) {
        if (membershipService.ValidatePasswordResetToken(token, user)) {
          authenticationService.LogIn(user.Username, false);
          return RedirectToAction("Edit", new { id = user.Id });
        }
      }
      // if we got here, the token is not valid
      TempData[GlobalViewDataProperty.PageErrorMessage] = "The reset password link that you clicked on is invalid, enter your information again";
      return RedirectToAction("ResetPassword");
    }

    public ActionResult Logout() {
      authenticationService.LogOut();
      return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public ActionResult SignUp() {
      var vm = new SignUpViewModel();
      vm.SiteName = configService.GetConfig().SiteName;
      return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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

      try {
        SendNewUserAwaitingApprovalEmail(user);
      }
      catch (System.Net.Mail.SmtpException e) {
        // log mail exception but don't let it interrupt the process
        ErrorSignal.FromCurrentContext().Raise(e);
      }
      repository.SaveChanges();
      return View("SignUpComplete");
    }

    protected void SendNewUserAwaitingApprovalEmail(User newUser) {
      var administrators = repository.All<Role>().Single(r => r.Name.Equals(Roles.Administrators)).Users;
      if (administrators.Any()) {
        var adminEmailAddresses = administrators.Select(u => u.Email).ToList();
        var siteName = configService.GetConfig().SiteName;
        var subject = string.Format("New user sign up at {0}", siteName);
        var body = new StringBuilder();
        body.AppendFormat("A new user has signed up at {0} and needs admin approval:{1}{1}", siteName, Environment.NewLine);
        body.AppendFormat("Username: {0}{1}Name: {2} {3}{1}Email: {4}", newUser.Username, Environment.NewLine, newUser.FirstName, newUser.LastName, newUser.Email);
        emailService.SendSystemEmail(adminEmailAddresses, subject, body.ToString());
      }
    }

    [Authorize(Roles=Roles.Administrators)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, int page, string q) {
      User userToDelete = repository.Get<User>(id);
      if (null == userToDelete) {
        return HttpNotFound();
      }
      if (CanDeleteUser(userToDelete, repository)) {
        repository.Delete(userToDelete);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was deleted successfully.";
      }
      else {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "There is data in the system referencing this user, the user cannot be deleted.";
      }
      return RedirectToAction("Index", new { page = page, q = q });
    }

    protected bool CanDeleteUser(User user, IRepository repository) {
      var results = repository.All<MatchResult>().Any(r => r.Player.Id == user.Id);
      return !results;
    }

    [HttpGet]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Unapproved() {
      var viewModel = new UnapprovedViewModel();
      viewModel.UnapprovedUsers = repository.All<User>().Where(u => !u.IsApproved).ToList()
        .Select(u => new UnapprovedUser(u));
      return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Approve(int[] userIds) {
      var users = repository.All<User>().Where(u => userIds.Contains(u.Id));
      if (users.Any()) {
        bool saveComplete = false;
        var siteName = configService.GetConfig().SiteName;
        var emailSubject = string.Format("{0} account approved", siteName);
        var helper = new UrlHelper(((MvcHandler)HttpContext.CurrentHandler).RequestContext);
        var url = helper.Action("Login", "Users", null, HttpContext.Request.Url.Scheme);
        var emailsToSend = new List<Tuple<string, string, string, string>>();
        foreach (var user in users) {
          user.IsApproved = true;
          var body = string.Format("Your {0} user account has been approved.{1}{1}Username: {2}{1}{1}Login here: {3}",
            siteName, Environment.NewLine, user.Username, url);
          emailsToSend.Add(new Tuple<string, string, string, string>(user.Email, emailSubject, body, user.FullName));
        }
        try {
          repository.SaveChanges();
          saveComplete = true;
        }
        catch (UpdateConcurrencyException) {
          TempData[GlobalViewDataProperty.PageErrorMessage] = "One or more of the selected users were updated by another user while you " +
            "were viewing this page. Enter your changes again.";
        }
        if (saveComplete) {
          var failedEmails = SendApprovedEmails(emailsToSend);
          if (failedEmails.Any()) {
            TempData["FailedEmails"] = failedEmails;
          }
          if (failedEmails.Count != emailsToSend.Count) {
            TempData[GlobalViewDataProperty.PageNotificationMessage] = "The selected users have been approved.";
          }
        }
      }
      return RedirectToAction("Unapproved");
    }

    private List<Tuple<string, string>> SendApprovedEmails(List<Tuple<string, string, string, string>> emails) {
      var failedEmails = new List<Tuple<string, string>>();
      foreach (var email in emails) {
        try {
          emailService.SendSystemEmail(email.Item1, email.Item2, email.Item3);
        }
        catch (System.Net.Mail.SmtpException e) {
          ErrorSignal.FromCurrentContext().Raise(e);
          failedEmails.Add(new Tuple<string, string>(email.Item4, email.Item1));
        }
      }
      return failedEmails;
    }

    [HttpGet]
    [Authorize]
    public ActionResult Edit(int id) {
      var user = repository.Get<User>(id);
      var currentPrincipal = authenticationService.GetCurrentPrincipal();
      var canEditUser = CanEditUser(currentPrincipal, user);
      if (!canEditUser) {
        return ErrorView("You are not authorized to edit this user");
      }
      var canEditStatus = CanEditUserStatus(currentPrincipal, user);
      var canEditRoles = CanEditUserRoles(currentPrincipal, user);
      var canEditPassword = CanEditUserPassword(currentPrincipal, user);
      var viewModel = new EditViewModel(user);
      viewModel.ShowStatus = canEditStatus;
      viewModel.ShowRoles = canEditRoles;
      viewModel.ShowPassword = canEditPassword;
      if (canEditRoles) {
        viewModel.LoadAvailableRoles(repository);
      }
      return View(viewModel);
    }


    // TODO: These 4 CanEdit* methods should really be in some type of service,
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

    protected bool CanEditUserPassword(ClubPoolPrincipal principal, User user) {
      // admins & self can edit password
      return principal.IsInRole(Roles.Administrators) || principal.UserId == user.Id;
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditViewModel viewModel) {
      var usernameChanged = false;
      var authTicketChanged = false;
      var previousUsername = "";
      try {
        var user = repository.Get<User>(viewModel.Id);
        if (null == user) {
          return ErrorView("The user you were editing was deleted by another user");
        }

        if (viewModel.Version != user.EncodedVersion) {
          return EditRedirectForConcurrency(viewModel.Id);
        }

        var currentPrincipal = authenticationService.GetCurrentPrincipal();
        var canEditUser = CanEditUser(currentPrincipal, user);
        if (!canEditUser) {
          return ErrorView("You are not authorized to edit this user");
        }
        var canEditStatus = CanEditUserStatus(currentPrincipal, user);
        var canEditRoles = CanEditUserRoles(currentPrincipal, user);
        var canEditPassword = CanEditUserPassword(currentPrincipal, user);
        var editingSelf = currentPrincipal.UserId == user.Id;

        // must reset these in case we redisplay the form
        viewModel.ShowStatus = canEditStatus;
        viewModel.ShowRoles = canEditRoles;
        viewModel.ShowPassword = canEditPassword;
        viewModel.LoadAvailableRoles(repository);


        if (!ModelState.IsValid) {
          return View(viewModel);
        }

        if (!user.Username.Equals(viewModel.Username)) {
          // verify that the new username is not in use
          if (membershipService.UsernameIsInUse(viewModel.Username)) {
            ModelState.AddModelErrorFor<EditViewModel>(m => m.Username, "The username is already in use");
            return View(viewModel);
          }
          previousUsername = user.Username;
          user.Username = viewModel.Username;
          usernameChanged = true;
        }
        if (!user.Email.Equals(viewModel.Email)) {
          // verify that the new email is not in use
          if (membershipService.EmailIsInUse(viewModel.Email)) {
            ModelState.AddModelErrorFor<EditViewModel>(m => m.Email, "The email address is already in use");
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
              user.AddRole(repository.Get<Role>(roleId));
            }
          }
        }
        if (canEditPassword && null != viewModel.Password && !string.IsNullOrEmpty(viewModel.Password.Trim())) {
          user.Password = membershipService.EncodePassword(viewModel.Password, user.PasswordSalt);
        }

        try {
          repository.SaveChanges();
        }
        catch (UpdateConcurrencyException) {
          return EditRedirectForConcurrency(viewModel.Id);
        }

        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was updated successfully";
        if (editingSelf && usernameChanged) {
          // we must update the auth ticket
          // using LogIn to do this will reset the persistent cookie
          // setting to false, which is not ideal but the other way
          // would be more complicated than its worth
          authenticationService.LogIn(user.Username, false);
          authTicketChanged = true;
        }
        return RedirectToAction("Edit", new { id = viewModel.Id });
      }
      catch (Exception) {
        // revert new auth ticket, if set
        if (authTicketChanged) {
          authenticationService.LogIn(previousUsername, false);
        }
        throw;
      }
    }

    private ActionResult EditRedirectForConcurrency(int id) {
      TempData[GlobalViewDataProperty.PageErrorMessage] =
        "This user was updated by another user while you were viewing this page. Enter your changes again.";
      return RedirectToAction("Edit", new { id = id });
    }

    [HttpGet]
    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Create() {
      var viewModel = new CreateViewModel();
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles=Roles.Administrators)]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CreateViewModel viewModel) {
      var user = CreateUser(viewModel, true, false);

      if (null == user) {
        // if we couldn't create the user that means there was some type of validation error,
        // so redisplay the form with the model
        return View(viewModel);
      }

      repository.SaveChanges();
      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was created successfully";
      return RedirectToAction("Index");
    }

    protected User CreateUser(CreateViewModel viewModel, bool approved, bool locked) {
      if (!ModelState.IsValid) {
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
    public ActionResult View(int id) {
      var user = repository.Get<User>(id);
      if (null == user) {
        return HttpNotFound();
      }

      var viewModel = new ViewViewModel(user, repository);
      var principal = authenticationService.GetCurrentPrincipal();
      viewModel.ShowAdminProperties = principal.IsInRole(Roles.Administrators);
      return View(viewModel);
    }

    [HttpGet]
    public ActionResult RecoverUsername() {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [CaptchaValidation("captcha")]
    public ActionResult RecoverUsername(RecoverUsernameViewModel viewModel, bool captchaValid) {
      if (!ModelState.IsValid) {
        return View(viewModel);
      }
      
      if (!captchaValid) {
        ModelState.AddModelError("captcha", "Incorrect. Try again.");
        return View(viewModel);
      }

      var usernames = repository.All<User>().Where(u => u.Email.Equals(viewModel.Email)).Select(u => u.Username).ToList();
      string body = "";
      if (!usernames.Any()) {
        body = string.Format("There are no usernames registered for the email address '{0}'.", viewModel.Email);
      }
      else {
        var bodysb = new StringBuilder(string.Format("The following usernames are registered for the email address '{0}':{1}", 
          viewModel.Email, Environment.NewLine));
        foreach (var username in usernames) {
          bodysb.Append(Environment.NewLine + username);
        }
        body = bodysb.ToString();
      }
      var siteName = configService.GetConfig().SiteName;
      emailService.SendSystemEmail(viewModel.Email, string.Format("{0} Username Assistance", siteName), body);
      return View("RecoverUsernameComplete");
    }
  }
}
