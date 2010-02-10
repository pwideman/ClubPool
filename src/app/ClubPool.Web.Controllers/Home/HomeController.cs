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
using ClubPool.Web.Controllers.Shared.SidebarGadgets;

namespace ClubPool.Web.Controllers
{
  public class HomeController : BaseController
  {
    public HomeController() {
    }

    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      var sidebarGadgetCollection = GetSidebarGadgetCollectionForIndex();
      ViewData[sidebarGadgetCollection.GetType().FullName] = sidebarGadgetCollection;
      return View(viewModel);
    }

    protected SidebarGadgetCollection GetSidebarGadgetCollectionForIndex() {
      var sidebarGadgetCollection = new SidebarGadgetCollection();
      if (!HttpContext.User.Identity.IsAuthenticated) {
        var loginGadget = new LoginSidebarGadget();
        sidebarGadgetCollection.Add(loginGadget.Name, loginGadget);
      }
      return sidebarGadgetCollection;
    }
  }
}
