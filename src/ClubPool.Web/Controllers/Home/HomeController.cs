using System;
using System.Web.Mvc;

using ClubPool.Web.Controllers.Shared.SidebarGadgets;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Infrastructure.Configuration;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Home
{
  public class HomeController : BaseController
  {
    private IAuthenticationService authenticationService;
    private ClubPoolConfiguration config;

    public HomeController(IAuthenticationService authSvc, ClubPoolConfiguration config) {
      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(config, "configService");

      authenticationService = authSvc;
      this.config = config;
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
