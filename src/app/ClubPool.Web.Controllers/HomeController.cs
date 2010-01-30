using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

using MvcContrib.Filters;
using Spark;
using SharpArch.Core.PersistenceSupport;

using ClubPool.Core;
using ClubPool.Web.Controllers.ViewModels;
using ClubPool.ApplicationServices.Interfaces;

namespace ClubPool.Web.Controllers
{
  public class HomeController : BaseController
  {
    public HomeController(IMembershipService membershipSvc, IRoleService roleSvc)
      : base(membershipSvc, roleSvc) {
    }

    public ActionResult Index() {
      var viewModel = new HomeIndexViewModel();
      InitializeViewModel(viewModel);
      return View(viewModel);
    }
  }
}
