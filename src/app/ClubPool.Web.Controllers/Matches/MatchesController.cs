using System;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using SharpArch.Core;
using SharpArch.Web.NHibernate;

using ClubPool.Web.Controllers.Matches.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Core;
using ClubPool.Core.Contracts;
using ClubPool.Core.Queries;

namespace ClubPool.Web.Controllers.Matches
{
  public class MatchesController : BaseController
  {
    [HttpPost]
    [Transaction]
    [Authorize]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditMatchViewModel viewModel) {
      return View();
    }
  }
}
