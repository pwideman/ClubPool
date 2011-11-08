using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using SharpArch.Core;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Controllers.Navigation.ViewModels;

namespace ClubPool.Web.Controllers.Navigation
{
  public class NavigationController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected ISeasonRepository seasonRepository;
    protected ITeamRepository teamRepository;

    public NavigationController(IAuthenticationService authSvc,
      ISeasonRepository seasonRepository,
      ITeamRepository teamRepository) {

      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != seasonRepository, "seasonRepository cannot be null");
      Check.Require(null != teamRepository, "teamRepository cannot be null");

      authenticationService = authSvc;
      this.seasonRepository = seasonRepository;
      this.teamRepository = teamRepository;
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
      if (seasonRepository.FindAll(s => s.IsActive).Any()) {
        viewModel.HasActiveSeason = true;
        var season = seasonRepository.FindOne(s => s.IsActive);
        viewModel.ActiveSeasonId = season.Id;
        viewModel.ActiveSeasonName = season.Name;
        if (viewModel.UserIsLoggedIn) {
          var team = teamRepository.FindOne(t => t.Division.Season == season && t.Players.Where(p => p.Id == viewModel.UserId).Any());
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
