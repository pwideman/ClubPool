using System;
using System.Web.Mvc;

using ClubPool.Web.Controllers.Shared.SidebarGadgets;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Services.Configuration;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Home
{
  public class HomeController : BaseController
  {
    private IAuthenticationService authenticationService;
    private IConfigurationService configService;

    public HomeController(IAuthenticationService authSvc, IConfigurationService configService) {
      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(configService, "configService");

      authenticationService = authSvc;
      this.configService = configService;
    }

    [HttpGet]
    public ActionResult Index() {
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View();
    }

    private SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      if (!authenticationService.IsLoggedIn()) {
        var loginGadget = new LoginSidebarGadget();
        sidebarGadgetCollection.Add(LoginSidebarGadget.Name, loginGadget);
      }
      return sidebarGadgetCollection;
    }

    [HttpGet]
    public ActionResult About() {
      return View();
    }

    [HttpGet]
    [Authorize]
    public ActionResult Rules() {
      return View();
    }

    [HttpGet]
    [Authorize]
    public ActionResult Regulations() {
      return View();
    }
  }
}
