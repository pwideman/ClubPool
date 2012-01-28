using System;
using System.Linq;
using System.Web.Mvc;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;
using ClubPool.Web.Services.Authentication;

namespace ClubPool.Web.Controllers.Navigation
{
  public class NavigationController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IRepository repository;

    public NavigationController(IAuthenticationService authSvc,
      IRepository repository) {

      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(repository, "repository");

      authenticationService = authSvc;
      this.repository = repository;
    }

    public ActionResult Menu() {
      var viewModel = new MenuViewModel();
      if (authenticationService.IsLoggedIn()) {
        var principal = authenticationService.GetCurrentPrincipal();
        viewModel.DisplayAdminMenu = principal.IsInRole(Roles.Administrators);
        viewModel.UserId = principal.UserId;
        viewModel.UserIsLoggedIn = true;
      }
      else {
        viewModel.DisplayAdminMenu = false;
        viewModel.UserIsLoggedIn = false;
      }
      var season = repository.All<Season>().SingleOrDefault(s => s.IsActive);
      if (null != season) {
        viewModel.HasActiveSeason = true;
        viewModel.ActiveSeasonId = season.Id;
        viewModel.ActiveSeasonName = season.Name;
        if (viewModel.UserIsLoggedIn) {
          var team = repository.All<Team>().SingleOrDefault(t => t.Division.Season.Id == season.Id && t.Players.Where(p => p.Id == viewModel.UserId).Any());
          if (null != team) {
            viewModel.HasCurrentTeam = true;
            viewModel.CurrentTeamId = team.Id;
          }
        }
      }
      else {
        viewModel.HasActiveSeason = false;
      }
      return PartialView(viewModel);
    }

  }
}
