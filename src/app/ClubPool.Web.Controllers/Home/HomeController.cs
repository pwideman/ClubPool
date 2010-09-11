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

namespace ClubPool.Web.Controllers.Home
{
  public class HomeController : BaseController
  {
    protected IAuthenticationService authenticationService;

    public HomeController(IAuthenticationService authSvc) {
      Check.Require(null != authSvc, "authSvc cannot be null");

      authenticationService = authSvc;
    }

    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[GlobalViewDataProperty.SidebarGadgetCollection] = sidebarGadgetCollection;
      return View(viewModel);
    }

    public ActionResult StaleObjectStateError() {
      return View();
    }

    protected SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      if (!authenticationService.IsLoggedIn()) {
        var loginGadget = new LoginSidebarGadget();
        sidebarGadgetCollection.Add(loginGadget.Name, loginGadget);
      }
      return sidebarGadgetCollection;
    }
  }
}
