using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.Web.Controllers.CurrentSeason.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers.CurrentSeason
{
  public class CurrentSeasonController : BaseController
  {
    protected ISeasonRepository seasonRepository;
    protected IDivisionRepository divisionRepository;

    public CurrentSeasonController(ISeasonRepository seasonRepo, IDivisionRepository divisionRepo) {
      Check.Require(null != seasonRepo, "seasonRepo cannot be null");
      Check.Require(null != divisionRepo, "divisionRepo cannot be null");

      seasonRepository = seasonRepo;
      divisionRepository = divisionRepo;
    }

    [Authorize]
    [HttpGet]
    [Transaction]
    public ActionResult Schedule() {
      var season = seasonRepository.GetAll().Where(s => s.IsActive).Single();
      if (null == season) {
        return ErrorView("There is no current season");
      }
      else {
        var viewModel = new CurrentSeasonScheduleViewModel(season);
        return View(viewModel);
      }
    }

  }
}
