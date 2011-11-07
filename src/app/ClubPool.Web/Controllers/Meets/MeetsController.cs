using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using SharpArch.Core;
using SharpArch.Web.NHibernate;

using ClubPool.Framework;
using ClubPool.Web.Infrastructure;
using ClubPool.Framework.Validation;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Meets.ViewModels;
using ClubPool.Web.Controllers.Extensions;

namespace ClubPool.Web.Controllers.Meets
{
  public class MeetsController : BaseController
  {
    protected IMeetRepository meetRepository;
    protected IAuthenticationService authService;
    protected IUserRepository userRepository;

    public MeetsController(IMeetRepository meetRepository, IAuthenticationService authSvc, IUserRepository userRepository) {
      Check.Require(null != meetRepository, "meetRepository cannot be null");
      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != userRepository, "userRepository cannot be null");

      this.meetRepository = meetRepository;
      this.authService = authSvc;
      this.userRepository = userRepository;
    }

    [Authorize]
    [Transaction]
    public ActionResult View(int id) {
      var meet = meetRepository.Get(id);
      if (null == meet) {
        return HttpNotFound();
      }

      var viewModel = new MeetViewModel(meet);
      var loggedInUser = userRepository.FindOne(u => u.Username.Equals(authService.GetCurrentPrincipal().Identity.Name));
      viewModel.AllowUserToEnterResults = meet.UserCanEnterMatchResults(loggedInUser);
      return View(viewModel);
    }

    [Authorize]
    [Transaction]
    public ActionResult Scoresheet(int id) {
      var meet = meetRepository.Get(id);
      var viewModel = new ScoresheetViewModel(meet);
      return View(viewModel);
    }
  }
}
