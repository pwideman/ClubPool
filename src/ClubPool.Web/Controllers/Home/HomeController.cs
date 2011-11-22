using System.Web.Mvc;

using ClubPool.Web.Controllers.Home.ViewModels;
using ClubPool.Web.Controllers.Shared.SidebarGadgets;
using ClubPool.Web.Services.Authentication;
using ClubPool.Web.Services.Configuration;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Home
{
  public class HomeController : BaseController
  {
    protected IAuthenticationService authenticationService;
    protected IConfigurationService configService;

    public HomeController(IAuthenticationService authSvc, IConfigurationService configService) {
      Arg.NotNull(authSvc, "authSvc");
      Arg.NotNull(configService, "configService");

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
