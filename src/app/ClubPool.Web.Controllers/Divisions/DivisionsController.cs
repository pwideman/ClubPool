﻿using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections.Generic;

using MvcContrib;
using MvcContrib.Pagination;
using SharpArch.Web.NHibernate;
using SharpArch.Core;
using xVal.ServerSide;

using ClubPool.Web.Controllers.Divisions.ViewModels;
using ClubPool.Web.Controllers.Extensions;
using ClubPool.Framework.Extensions;
using ClubPool.Framework.Validation;
using ClubPool.Framework.NHibernate;
using ClubPool.Core;
using ClubPool.Core.Queries;
using ClubPool.Web.Controllers.Attributes;

namespace ClubPool.Web.Controllers.Divisions
{

  public class DivisionsController : BaseController
  {
    protected ILinqRepository<Season> seasonRepository;
    protected ILinqRepository<Division> divisionRepository;

    public DivisionsController(ILinqRepository<Division> divisionRepo, ILinqRepository<Season> seasonRepo) {
      Check.Require(null != seasonRepo, "seasonRepo cannot be null");
      Check.Require(null != divisionRepo, "divisionRepo cannot be null");

      seasonRepository = seasonRepo;
      divisionRepository = divisionRepo;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Create(int seasonId) {
      var season = seasonRepository.Get(seasonId);
      if (null == season) {
        throw new HttpException((int)HttpStatusCode.NotFound, "The requested resource is not found");
      }
      var dto = new DivisionViewModel() {
        SeasonId = season.Id,
        SeasonName = season.Name,
      };
      return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Create(DivisionViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }
      DateTime startingDate;
      if (!DateTime.TryParse(viewModel.StartingDate, out startingDate)) {
        ModelState.AddModelErrorFor<DivisionViewModel>(m => m.StartingDate, "Enter a valid date");
        return View(viewModel);
      }

      Season s = seasonRepository.Get(viewModel.SeasonId);
      Division d = new Division(viewModel.Name, startingDate);
      divisionRepository.SaveOrUpdate(d);
      s.AddDivision(d);
      seasonRepository.SaveOrUpdate(s);

      // I hate doing this here because theoretically /divisions/create could be called
      // from anywhere, but I don't know what else to do now. Same comment applies for all
      // redirects in this controller
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(s.Id));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id) {
      var division = divisionRepository.Get(id);
      if (null == division) {
        throw new HttpException((int)HttpStatusCode.NotFound, "The requested resource is not found");
      }
      var seasonId = division.Season.Id;
      if (!division.CanDelete()) {
        TempData[GlobalViewDataProperty.PageErrorMessage] = "The division cannot be deleted";
      }
      else {
        divisionRepository.Delete(division);
        TempData[GlobalViewDataProperty.PageNotificationMessage] = "The division was deleted";
      }
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(seasonId));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    public ActionResult Edit(int id) {
      var division = divisionRepository.Get(id);
      if (null == division) {
        throw new HttpException((int)HttpStatusCode.NotFound, "The requested resource is not found");
      }
      var viewModel = new DivisionViewModel();
      viewModel.Id = id;
      viewModel.Name = division.Name;
      viewModel.SeasonId = division.Season.Id;
      viewModel.SeasonName = division.Season.Name;
      viewModel.StartingDate = division.StartingDate.ToShortDateString();
      return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrators)]
    [Transaction]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(DivisionViewModel viewModel) {
      if (!ValidateViewModel(viewModel)) {
        return View(viewModel);
      }
      DateTime startingDate;
      if (!DateTime.TryParse(viewModel.StartingDate, out startingDate)) {
        ModelState.AddModelErrorFor<DivisionViewModel>(m => m.StartingDate, "Enter a valid date");
        return View(viewModel);
      }

      var division = divisionRepository.Get(viewModel.Id);
      division.Name = viewModel.Name;
      division.StartingDate = startingDate;
      divisionRepository.SaveOrUpdate(division);

      TempData[GlobalViewDataProperty.PageNotificationMessage] = "The division was updated";
      return this.RedirectToAction<Seasons.SeasonsController>(c => c.View(viewModel.SeasonId));
    }
  }

}
