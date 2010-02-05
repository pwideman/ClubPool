using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

using MvcContrib.Filters;
using Spark;
using SharpArch.Core.PersistenceSupport;

using ClubPool.Core;
using ClubPool.Web.Controllers.Home.ViewModels;
using ClubPool.ApplicationServices.Interfaces;
using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.Home
{
  [Precompile("_TestPanel")]
  public class HomeController : BaseController
  {
    public HomeController() {
    }

    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      ViewData["SidebarPanels"] = GetSidebarViewDataForIndex();
      return View(viewModel);
    }

    protected IList<SidebarPanelViewData> GetSidebarViewDataForIndex() {
      var sidebarViewData = new SidebarPanelViewData() {
        Name = "_TestPanel",
        ViewModel = new TestPanelViewModel() { Title = "Test Sidebar Panel" }
      };
      return new List<SidebarPanelViewData>() { sidebarViewData };
    }
  }
}
