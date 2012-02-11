using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Elmah;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Infrastructure.Configuration;
using ClubPool.Web.Services.Messaging;

namespace ClubPool.Web.Controllers.UnapprovedUsers
{
  public class UnapprovedUsersController : BaseController, IRouteRegistrar
  {
    private IRepository repository;
    private IEmailService emailService;
    private ClubPoolConfiguration config;

    public UnapprovedUsersController(IRepository repository, IEmailService emailService, ClubPoolConfiguration config) {
      this.repository = repository;
      this.emailService = emailService;
      this.config = config;
    }

    public void RegisterRoutes(RouteCollection routes) {
      routes.MapRoute("unapprovedusers", "unapprovedusers", new { Controller = "UnapprovedUsers", Action = "UnapprovedUsers" });
      routes.MapRoute("approveusers", "approveusers", new { Controller = "UnapprovedUsers", Action = "ApproveUsers" });
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult UnapprovedUsers() {
      var viewModel = new UnapprovedUsersViewModel();
      viewModel.UnapprovedUsers = repository.All<User>().Where(u => !u.IsApproved).ToList()
        .Select(u => new UnapprovedUser() {
          Id = u.Id,
          Email = u.Email,
          Name = u.FullName
        });
      return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Administrators)]
    public ActionResult ApproveUsers(int[] userIds) {
      var users = repository.All<User>().Where(u => userIds.Contains(u.Id));
      if (users.Any()) {
        bool saveComplete = false;
        var siteName = config.SiteName;
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
      return RedirectToAction("UnapprovedUsers");
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
  }
}
