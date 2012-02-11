using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Web;

using Elmah;

using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controls.Captcha;
using ClubPool.Web.Infrastructure.Configuration;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Services.Messaging;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Web.Models;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Users
{
  public class UsersController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IMembershipService membershipService;
    protected IRepository repository;
    protected IEmailService emailService;
    protected ClubPoolConfiguration config;

    public UsersController(IAuthenticationService authSvc, 
      IMembershipService membershipSvc, 
      IEmailService emailSvc,
      ClubPoolConfiguration config,
      IRepository repository)
    {

      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(membershipSvc, "membershipSvc");
      Arg.NotNull(emailSvc, "emailSvc");
      Arg.NotNull(config, "config");
      Arg.NotNull(repository, "repository");

      authenticationService = authSvc;
      membershipService = membershipSvc;
      emailService = emailSvc;
      this.config = config;
      this.repository = repository;
    }

    [Authorize(Roles=Roles.Administrators)]
    public ActionResult Index(int? page, string q) {
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
      var viewModel = BuildIndexViewModel(userQuery, page.GetValueOrDefault(1));
      if (!string.IsNullOrEmpty(q)) {
        viewModel.SearchQuery = q;
      }
      return View(viewModel);
    }

    private IndexViewModel BuildIndexViewModel(IQueryable<User> query, int page) {
      var model = new IndexViewModel();
      InitializePagedListViewModel(model, query, page, 10, (u) => new UserSummaryViewModel() {
        Id = u.Id,
        Name = u.FullName,
        Username = u.Username,
        Email = u.Email,
        IsApproved = u.IsApproved,
        IsLocked = u.IsLocked,
        Roles = u.Roles.Select(r => r.Name).ToArray()
      });
      return model;
    }

    [HttpGet]
    public ActionResult SignUp() {
      return View(new CreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [CaptchaValidation("captcha")]
    public ActionResult SignUp(CreateViewModel viewModel, bool captchaValid) {
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
      return RedirectToAction("SignUpComplete");
    }

    [HttpGet]
    public ActionResult SignUpComplete() {
      return View();
    }

    protected void SendNewUserAwaitingApprovalEmail(User newUser) {
      var administrators = repository.All<Role>().Single(r => r.Name.Equals(Roles.Administrators)).Users;
      if (administrators.Any()) {
        var adminEmailAddresses = administrators.Select(u => u.Email).ToList();
        var siteName = config.SiteName;
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
    [Authorize]
    public ActionResult Edit(int id) {
      var user = repository.Get<User>(id);
      var currentPrincipal = authenticationService.GetCurrentPrincipal();
      var canEditUser = CanEditUser(currentPrincipal, user);
      if (!canEditUser) {
        return ErrorView("You are not authorized to edit this user");
      }
      var viewModel = BuildEditViewModel(user, currentPrincipal);
      return View(viewModel);
    }

    private EditViewModel BuildEditViewModel(User user, ClubPoolPrincipal currentPrincipal) {
      var model = new EditViewModel {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        IsApproved = user.IsApproved,
        IsLocked = user.IsLocked,
        Username = user.Username,
        Version = user.EncodedVersion,
        Roles = user.Roles.Select(r => r.Id).ToArray()
      };
      model.ShowStatus = CanEditUserStatus(currentPrincipal, user);
      model.ShowRoles = CanEditUserRoles(currentPrincipal, user);
      if (model.ShowRoles) {
        model.AvailableRoles = repository.All<Role>().Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToList();
      }
      model.ShowPassword = CanEditUserPassword(currentPrincipal, user);
      return model;
    }

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
        if (!CanEditUser(currentPrincipal, user)) {
          return ErrorView("You are not authorized to edit this user");
        }

        if (!ModelState.IsValid) {
          return View(viewModel);
        }

        var canEditStatus = CanEditUserStatus(currentPrincipal, user);
        var canEditRoles = CanEditUserRoles(currentPrincipal, user);
        var canEditPassword = CanEditUserPassword(currentPrincipal, user);
        previousUsername = user.Username;
        if (!TryUpdateUser(user, viewModel, canEditStatus, canEditRoles, canEditPassword)) {
          viewModel.ShowStatus = canEditStatus;
          viewModel.ShowRoles = canEditRoles;
          if (canEditRoles) {
            viewModel.AvailableRoles = repository.All<Role>().Select(r => new RoleViewModel { Id = r.Id, Name = r.Name }).ToList();
          }
          viewModel.ShowPassword = canEditPassword;
          return View(viewModel);
        }

        try {
          repository.SaveChanges();
        }
        catch (UpdateConcurrencyException) {
          return EditRedirectForConcurrency(viewModel.Id);
        }

        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The user was updated successfully";
        if (currentPrincipal.UserId == user.Id && previousUsername != user.Username) {
          // if editing self and updating the username, we must update the auth ticket
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

    private bool TryUpdateUser(User user, EditViewModel viewModel, bool canEditStatus, bool canEditRoles, bool canEditPassword) {
      if (!user.Username.Equals(viewModel.Username)) {
        // verify that the new username is not in use
        if (membershipService.UsernameIsInUse(viewModel.Username)) {
          ModelState.AddModelErrorFor<EditViewModel>(m => m.Username, "The username is already in use");
          return false;
        }
        user.Username = viewModel.Username;
      }
      if (!user.Email.Equals(viewModel.Email)) {
        // verify that the new email is not in use
        if (membershipService.EmailIsInUse(viewModel.Email)) {
          ModelState.AddModelErrorFor<EditViewModel>(m => m.Email, "The email address is already in use");
          return false;
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
      return true;
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
    public ActionResult Details(int id) {
      var user = repository.Get<User>(id);
      if (null == user) {
        return HttpNotFound();
      }

      var viewModel = BuildDetailsViewModel(user);
      var principal = authenticationService.GetCurrentPrincipal();
      viewModel.ShowAdminProperties = principal.IsInRole(Roles.Administrators);
      return View(viewModel);
    }

    private DetailsViewModel BuildDetailsViewModel(User user) {
      var model = new DetailsViewModel {
        Id = user.Id,
        Username = user.Username,
        Name = user.FullName,
        Email = user.Email,
        IsApproved = user.IsApproved,
        IsLocked = user.IsLocked,
        Roles = user.Roles.Select(r => r.Name).ToArray()
      };
      if (user.SkillLevels.Any()) {
        model.SkillLevel = user.SkillLevels.Single(sl => sl.GameType == GameType.EightBall).Value;
        model.SkillLevelCalculation = new SkillLevelCalculationViewModel(user, repository);
        model.HasSkillLevel = true;
      }
      else {
        model.HasSkillLevel = false;
      }
      return model;
    }

    [HttpGet]
    [Authorize]
    public ActionResult VerifySkillLevels() {
      var users = repository.All<User>();
      var viewModel = new VerifySkillLevelsViewModel();
      var updates = new List<SkillLevelUpdateViewModel>();
      foreach (var user in users) {
        var oldsl = user.SkillLevels.SingleOrDefault(sl => sl.GameTypeValue == (int)GameType.EightBall);
        if (null != oldsl) {
          var oldslv = oldsl.Value;
          user.UpdateSkillLevel(GameType.EightBall, repository);
          var newsl = user.SkillLevels.Single(sl => sl.GameTypeValue == (int)GameType.EightBall).Value;
          if (oldslv != newsl) {
            updates.Add(new SkillLevelUpdateViewModel { Name = user.FullName, NewSkillLevel = newsl, PreviousSkillLevel = oldslv });
          }
        }
      }
      if (updates.Any()) {
        repository.SaveChanges();
        viewModel.HasUpdates = true;
        viewModel.Updates = updates;
      }
      return View(viewModel);
    }
  }
}
