using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;

using ClubPool.Web.Controllers;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Controls.Captcha;
using ClubPool.Web.Models;
using ClubPool.Web.Services.Configuration;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Services.Messaging;

namespace ClubPool.Web.Controllers.AccountHelp
{
  public class AccountHelpController : BaseController, IRouteRegistrar
  {
    private IAuthenticationService authenticationService;
    private IMembershipService membershipService;
    private IRepository repository;
    private IEmailService emailService;
    private IConfigurationService configService;

    public AccountHelpController(IAuthenticationService authSvc,
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

    public void RegisterRoutes(RouteCollection routes) {
      routes.MapRoute("accounthelp", "accounthelp", new { Controller = "AccountHelp", Action = "AccountHelp" });
    }

    [HttpGet]
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
        var url = helper.Action("ValidatePasswordResetToken", "AccountHelp", new { token = token }, HttpContext.Request.Url.Scheme);
        var body = string.Format("You have requested to reset your password at {0}. Click the following link to be logged into your" +
          " account and taken to the member info page, where you can change your password. The link is valid for 24 hours.{1}{1}" +
          "WARNING: IF YOU DID NOT REQUEST THIS EMAIL, DO NOT CLICK THE LINK BELOW, AND ALERT THE SITE ADMINISTRATOR{1}{1}{2}",
          siteName, Environment.NewLine, url);
        emailService.SendSystemEmail(user.Email, string.Format("{0} password reset", siteName), body);
      }
      // always go to the complete page, we don't want potential attackers to be able to
      // discover valid usernames through this interface
      return RedirectToAction("ResetPasswordComplete");
    }

    [HttpGet]
    public ActionResult ResetPasswordComplete() {
      return View();
    }

    [HttpGet]
    public ActionResult ValidatePasswordResetToken(string token) {
      var users = repository.All<User>();
      foreach (var user in users) {
        if (membershipService.ValidatePasswordResetToken(token, user)) {
          authenticationService.LogIn(user.Username, false);
          return RedirectToAction("Edit", "Users", new { id = user.Id });
        }
      }
      // if we got here, the token is not valid
      TempData[GlobalViewDataProperty.PageErrorMessage] = "The reset password link that you clicked on is invalid, enter your information again";
      return RedirectToAction("ResetPassword");
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
      return RedirectToAction("RecoverUsernameComplete");
    }

    [HttpGet]
    public ActionResult RecoverUsernameComplete() {
      return View();
    }

  }
}
