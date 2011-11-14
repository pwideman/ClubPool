using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using ClubPool.Framework.Validation;
using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Meets.ViewModels;
using ClubPool.Web.Controllers.Extensions;

namespace ClubPool.Web.Controllers.Meets
{
  public class MeetsController : BaseController
  {
    protected IRepository repository;
    protected IAuthenticationService authService;

    public MeetsController(IRepository repository, IAuthenticationService authSvc) {
      Arg.NotNull(repository, "repository");
      Arg.NotNull(authSvc, "authSvc");

      this.repository = repository;
      this.authService = authSvc;
    }

    [Authorize]
    public ActionResult View(int id) {
      var meet = repository.Get<Meet>(id);
      if (null == meet) {
        return HttpNotFound();
      }

      var viewModel = new MeetViewModel(meet);
      var username = authService.GetCurrentPrincipal().Identity.Name;
      var loggedInUser = repository.All<User>().Single(u => u.Username.Equals(username));
      viewModel.AllowUserToEnterResults = meet.UserCanEnterMatchResults(loggedInUser);
      return View(viewModel);
    }

    [Authorize]
    public ActionResult Scoresheet(int id) {
      var meet = repository.Get<Meet>(id);
      var viewModel = new ScoresheetViewModel(meet);
      return View(viewModel);
    }
  }
}
