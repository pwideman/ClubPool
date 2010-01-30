using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

using MvcContrib.Filters;
using Spark;
using SharpArch.Core.PersistenceSupport;
using ClubPool.Core;

namespace ClubPool.Web.Controllers
{
  public class HomeIndexViewModel : ViewModelBase
  {
    public string Username { get; set; }
  }

  public class HomeController : BaseController
  {
    public ActionResult Index() {
      var viewModel = new HomeIndexViewModel();
      if (HttpContext.User.Identity.IsAuthenticated) {
        viewModel.Username = HttpContext.User.Identity.Name;
      }
      else {
        viewModel.Username = "Anonymous";
      }
      return View(viewModel);
    }
  }
}
