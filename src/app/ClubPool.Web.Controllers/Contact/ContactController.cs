using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using SharpArch.Core;
using SharpArch.Web.NHibernate;

using ClubPool.Web.Controllers.Contact.ViewModels;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Messaging.Contracts;

namespace ClubPool.Web.Controllers.Contact
{
  public class ContactController : BaseController
  {
    protected ITeamRepository teamRepository;
    protected IUserRepository userRepository;
    protected IAuthenticationService authenticationService;
    protected IEmailService emailService;

    public ContactController(IUserRepository userRepository,
      ITeamRepository teamRepository, 
      IAuthenticationService authenticationService,
      IEmailService emailService) {

      Check.Require(userRepository != null, "userRepository cannot be null");
      Check.Require(teamRepository != null, "teamRepository cannot be null");
      Check.Require(authenticationService != null, "authenticationService cannot be null");
      Check.Require(emailService != null, "emailService cannot be null");

      this.userRepository = userRepository;
      this.authenticationService = authenticationService;
      this.teamRepository = teamRepository;
      this.emailService = emailService;
    }

    [HttpGet]
    [Authorize]
    [Transaction]
    public ActionResult Team(int id) {
      var team = teamRepository.Get(id);
      if (null == team) {
        return HttpNotFound();
      }
      var sender = userRepository.Get(authenticationService.GetCurrentPrincipal().UserId);
      var viewModel = new TeamViewModel(team, sender);
      return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    [Transaction]
    public ActionResult Team(TeamViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }

      var team = teamRepository.Get(viewModel.TeamId);
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
  }
}
