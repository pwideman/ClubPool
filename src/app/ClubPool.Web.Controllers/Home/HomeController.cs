using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

using MvcContrib.Filters;
using Spark;
using SharpArch.Core.PersistenceSupport;

using ClubPool.Core;
using ClubPool.Web.Controllers.Home.ViewModels;
using ClubPool.ApplicationServices.Interfaces;

namespace ClubPool.Web.Controllers.Home
{
  public class HomeController : BaseController
  {
    public HomeController() {
    }

    public ActionResult Index() {
      var viewModel = new IndexViewModel();
      return View(viewModel);
    }
  }
}
