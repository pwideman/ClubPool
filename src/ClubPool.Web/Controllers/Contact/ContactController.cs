using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Services.Messaging;
using ClubPool.Web.Controllers.Contact.ViewModels;

namespace ClubPool.Web.Controllers.Contact
{
  public class ContactController : BaseController
  {
    protected IRepository repository;
    protected IAuthenticationService authenticationService;
    protected IEmailService emailService;

    public ContactController(IRepository repository,
      IAuthenticationService authenticationService,
      IEmailService emailService) {

      Arg.NotNull(repository, "repository");
      Arg.NotNull(authenticationService, "authenticationService");
      Arg.NotNull(emailService, "emailService");

      this.repository = repository;
      this.authenticationService = authenticationService;
      this.emailService = emailService;
    }

    [HttpGet]
    [Authorize]
    public ActionResult Team(int id) {
      var team = repository.Get<Team>(id);
      if (null == team) {
        return HttpNotFound();
      }
      var sender = repository.Get<User>(authenticationService.GetCurrentPrincipal().UserId);
      var viewModel = new ContactViewModel(team, sender);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    public ActionResult Team(ContactViewModel viewModel) {
      if (!ModelState.IsValid) {
        return View(viewModel);
      }

      var team = repository.Get<Team>(viewModel.Id);
      if (null == team) {
        return HttpNotFound();
      }

      var addresses = team.Players.Select(p => p.Email).ToList();
      emailService.SendEmail(viewModel.ReplyToAddress,
        addresses, 
        new List<string>() { viewModel.ReplyToAddress },
        null,
        viewModel.Subject,
        viewModel.Body);

      return View("EmailSuccess");
    }

    [HttpGet]
    [Authorize]
    public ActionResult Player(int id) {
      var player = repository.Get<User>(id);
      if (null == player) {
        return HttpNotFound();
      }
      var sender = repository.Get<User>(authenticationService.GetCurrentPrincipal().UserId);
      var viewModel = new ContactViewModel(player, sender);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    public ActionResult Player(ContactViewModel viewModel) {
      if (!ModelState.IsValid) {
        return View(viewModel);
      }

      var player = repository.Get<User>(viewModel.Id);
      if (null == player) {
        return HttpNotFound();
      }

      var to = player.Email;
      emailService.SendEmail(viewModel.ReplyToAddress,
        new List<string>() { to },
        new List<string>() { viewModel.ReplyToAddress },
        null,
        viewModel.Subject,
        viewModel.Body);

      return View("EmailSuccess");
    }
  }
}
