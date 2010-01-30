using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

using MvcContrib.Filters;
using Spark;
using SharpArch.Core.PersistenceSupport;

using ClubPool.Core;
using ClubPool.Web.Controllers.ViewModels;

namespace ClubPool.Web.Controllers
{
  public class HomeController : BaseController
  {
    public ActionResult Index() {
      var viewModel = new HomeIndexViewModel();
      if (HttpContext.User.Identity.IsAuthenticated) {
        viewModel.IsLoggedIn = true;
        viewModel.Username = HttpContext.User.Identity.Name;
      }
      return View(viewModel);
    }
  }
}
