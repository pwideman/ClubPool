using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using SharpArch.Core;

using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.ApplicationServices.Membership.Contracts;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.Web.Controllers.Navigation.ViewModels;

namespace ClubPool.Web.Controllers.Navigation
{
  public class NavigationController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected ISeasonRepository seasonRepository;

    public NavigationController(IAuthenticationService authSvc, ISeasonRepository seasonRepository) {
      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != seasonRepository, "seasonRepository cannot be null");

      authenticationService = authSvc;
      this.seasonRepository = seasonRepository;
    }

    public ActionResult Menu() {
      var viewModel = new MenuViewModel();
      if (authenticationService.IsLoggedIn()) {
        var principal = authenticationService.GetCurrentPrincipal();
        viewModel.DisplayAdminMenu = principal.IsInRole(Roles.Administrators);
        viewModel.UserIsLoggedIn = true;
      }
      else {
        viewModel.DisplayAdminMenu = false;
        viewModel.UserIsLoggedIn = false;
      }
      if (seasonRepository.FindAll(s => s.IsActive).Any()) {
        viewModel.HasActiveSeason = true;
        viewModel.ActiveSeasonId = seasonRepository.FindOne(s => s.IsActive).Id;
      }
      else {
        viewModel.HasActiveSeason = false;
      }
      return PartialView(viewModel);
    }

  }
}
