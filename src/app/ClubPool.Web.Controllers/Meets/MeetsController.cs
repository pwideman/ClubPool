using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using SharpArch.Core;
using SharpArch.Web.NHibernate;

using ClubPool.Web.Controllers.Meets.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;

namespace ClubPool.Web.Controllers.Meets
{
  public class MeetsController : BaseController
  {
    protected IMeetRepository meetRepository;

    public MeetsController(IMeetRepository meetRepository) {
      Check.Require(null != meetRepository, "meetRepository cannot be null");

      this.meetRepository = meetRepository;
    }

    [Authorize]
    [Transaction]
    public ActionResult View(int id) {
      var meet = meetRepository.Get(id);
      var viewModel = new MeetViewModel(meet);
      return View(viewModel);
    }

    [Authorize]
    [Transaction]
    public ActionResult Scoresheet(int id) {
      var meet = meetRepository.Get(id);
      var viewModel = new ScoresheetViewModel(meet);
      return View(viewModel);
    }
  }
}
