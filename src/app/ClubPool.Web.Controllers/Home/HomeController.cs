using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

using SharpArch.Core.PersistenceSupport;
using SharpArch.Core;

using ClubPool.Core;
using ClubPool.Web.Controllers.Home.ViewModels;
using ClubPool.Web.Controllers.Shared.SidebarGadgets;
using ClubPool.ApplicationServices.Authentication.Contracts;
using ClubPool.ApplicationServices.Configuration.Contracts;

namespace ClubPool.Web.Controllers.Home
{
  public class HomeController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IConfigurationService configService;

    public HomeController(IAuthenticationService authSvc, IConfigurationService configService) {
      Check.Require(null != authSvc, "authSvc cannot be null");
      Check.Require(null != configService, "configService cannot be null");

      authenticationService = authSvc;
      this.configService = configService;
    }

    [HttpGet]
    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      viewModel.SiteName = configService.GetConfig().SiteName;
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View(viewModel);
    }

    [HttpGet]
    public ActionResult StaleObjectStateError() {
      return View();
    }

    protected SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      if (!authenticationService.IsLoggedIn()) {
        var loginGadget = new LoginSidebarGadget();
        sidebarGadgetCollection.Add(LoginSidebarGadget.Name, loginGadget);
      }
      return sidebarGadgetCollection;
    }

    [HttpGet]
    public ActionResult About() {
      var viewModel = new AboutViewModel();
      viewModel.SiteName = configService.GetConfig().SiteName;
      return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public ActionResult Rules() {
      var viewModel = new RulesViewModel();
      viewModel.SiteName = configService.GetConfig().SiteName;
      return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public ActionResult Regulations() {
      var viewModel = new RegulationsViewModel();
      viewModel.SiteName = configService.GetConfig().SiteName;
      return View(viewModel);
    }
  }
}
