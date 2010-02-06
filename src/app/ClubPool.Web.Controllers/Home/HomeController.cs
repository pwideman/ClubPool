using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

using SharpArch.Core.PersistenceSupport;

using ClubPool.Core;
using ClubPool.ApplicationServices.Interfaces;
using ClubPool.Web.Controllers.Home.ViewModels;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers
{
  public class HomeController : BaseController
  {
    public HomeController() {
    }

    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      var sidebarCollection = GetSidebarCollectionForIndex();
      ViewData[sidebarCollection.GetType().FullName] = sidebarCollection;
      return View(viewModel);
    }

    protected SidebarCollection GetSidebarCollectionForIndex() {
      var sidebarCollection = new SidebarCollection();
      if (!HttpContext.User.Identity.IsAuthenticated) {
        var loginControlRequest = new PartialRequest();
        loginControlRequest.SetAction<ClubPool.Web.Controllers.UserController>(c => c.LoginGadget());
        var loginViewData = new SidebarPanelViewData() {
          Name = "Login",
          Action = loginControlRequest
        };
        sidebarCollection.Add(loginViewData);
      }
      return sidebarCollection;
    }
  }
}
