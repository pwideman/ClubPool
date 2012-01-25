using System;
using System.Web.Mvc;

using ClubPool.Web.Models;
using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Controllers.Scoresheet
{
  public class ScoresheetController : Controller
  {
    private IRepository repository;

    public ScoresheetController(IRepository repository) {
      Arg.NotNull(repository, "repository");
      this.repository = repository;
    }

    [Authorize]
    public ActionResult Index(int id) {
      var meet = repository.Get<Meet>(id);
      var viewModel = new ScoresheetViewModel(meet);
      return View(viewModel);
    }
  }
}
